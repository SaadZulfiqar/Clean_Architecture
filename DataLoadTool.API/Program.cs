using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.S3;
using DataLoadTool.Application.Services;
using DataLoadTool.Application.UseCases;
using DataLoadTool.Core.Interfaces;
using DataLoadTool.Infrastructure.FileStorage;
using DataLoadTool.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT configuration
ConfigureJWT(builder.Services);

// Load configuration
ConfigureConfiguration(builder.Configuration, builder.Environment.EnvironmentName);

// Configure logging
builder.Services.AddLogging(config => config.AddConsole());

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

// Load AWS options from configuration and set up services
var credentials = new BasicAWSCredentials(
    builder.Configuration["AWS:AccessKeyId"],
    builder.Configuration["AWS:SecretAccessKey"]
);

ConfigureAWSOptions(builder.Services, builder.Configuration, credentials, logger);

// Register application services and use cases
RegisterServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // middleware to authorize requests
app.UseAuthorization(); // middleware to authenticate requests
app.MapControllers();

await RunMigrationsAndSeeding(app, logger, builder.Configuration);

app.Run();

void ConfigureJWT(IServiceCollection services)
{
    var key = builder.Configuration["SecretKeyForSigningTokens"];
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });
    services.AddAuthorization();
}

void ConfigureConfiguration(IConfigurationBuilder configBuilder, string environment)
{
    configBuilder
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
}

void ConfigureAWSOptions(IServiceCollection services, IConfiguration configuration, AWSCredentials credentials, ILogger logger)
{
    var dynamodb_awsOptions = configuration.GetAWSOptions();
    var s3_awsOptions = configuration.GetAWSOptions();

    dynamodb_awsOptions.Credentials = credentials;
    s3_awsOptions.Credentials = credentials;

    var dynamoDbServiceUrl = configuration.GetValue<string>("AWS:DynamoDBLocalServiceURL");
    if (!string.IsNullOrEmpty(dynamoDbServiceUrl))
    {
        logger.LogInformation("DynamoDB Local configuration found. ServiceURL: {ServiceURL}", dynamoDbServiceUrl);
        dynamodb_awsOptions.DefaultClientConfig.ServiceURL = dynamoDbServiceUrl;
        services.AddSingleton<IAmazonDynamoDB>(sp =>
        {
            var clientConfig = new AmazonDynamoDBConfig { ServiceURL = dynamoDbServiceUrl };
            return new AmazonDynamoDBClient(credentials, clientConfig);
        });
    }
    else
    {
        logger.LogInformation("DynamoDB Local configuration not found. Using default AWS configuration.");
        services.AddDefaultAWSOptions(dynamodb_awsOptions);
        services.AddAWSService<IAmazonDynamoDB>();
    }

    logger.LogInformation("Setting up S3.");
    services.AddDefaultAWSOptions(s3_awsOptions);
    services.AddAWSService<IAmazonS3>();
}

void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    // Register DynamoDB context and repository
    services.AddSingleton<IDynamoDBContextWithPrefix>(provider =>
    {
        var client = provider.GetRequiredService<IAmazonDynamoDB>();
        var defaultPrefix = configuration["AWS:DynamoDB:TablePrefix"];
        return new DynamoDBContextWithPrefix(client, defaultPrefix);
    });

    // Register application services
    services.AddSingleton<ITokenService, TokenService>();
    services.AddSingleton<IS3FileStorageService, S3FileStorageService>();
    services.AddSingleton<IMigrationService, DynamoDBMigrationService>();
    services.AddSingleton<ISeedService, DynamoDBSeedService>();
    services.AddSingleton<IUploadService, UploadService>();
    services.AddSingleton<IDownloadService, DownloadService>();
    services.AddSingleton<IUserService, UserService>();
    services.AddSingleton<ITenantService, TenantService>();
    services.AddSingleton<IBatchService, BatchService>();
    services.AddSingleton<IBatchFileService, BatchFileService>();
    services.AddSingleton<ISettingService, SettingService>();
    services.AddSingleton<ISuperUserService, SuperUserService>();
    services.AddSingleton<IValidateCustomerDataUseCase, ValidateCustomerDataUseCase>();
    services.AddSingleton<ITransformCustomerDataUseCase, TransformCustomerDataUseCase>();

    // Register use cases
    services.AddSingleton<IUploadCustomerDataUseCase, UploadCustomerDataUseCase>();
}

async Task RunMigrationsAndSeeding(WebApplication app, ILogger logger, IConfiguration configuration)
{
    using (var scope = app.Services.CreateScope())
    {
        logger.LogInformation("Running migrations.");
        var services = scope.ServiceProvider;
        var migrationService = services.GetRequiredService<IMigrationService>();
        // await migrationService.RemoveAsync();
        await migrationService.MigrateAsync();

        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Running seed data.");
            var dynamoDbServiceUrl = configuration.GetValue<string>("AWS:DynamoDBLocalServiceURL");
            if (string.IsNullOrEmpty(dynamoDbServiceUrl))
            {
                // Give a delay for 20 seconds: that is because Dynamo DB takes time to publish tables. 
                await Task.Delay(20000);
            }
            var seedService = services.GetRequiredService<ISeedService>();
            await seedService.SeedAsync();
        }
    }
}