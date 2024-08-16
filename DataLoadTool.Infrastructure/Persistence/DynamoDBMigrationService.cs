using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using DataLoadTool.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace DataLoadTool.Infrastructure.Persistence
{
    public class DynamoDBMigrationService : IMigrationService
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private readonly List<Type> _entityTypes;
        private readonly string _defaultPrefix;

        public DynamoDBMigrationService(IAmazonDynamoDB dynamoDBClient, IConfiguration configuration)
        {
            _dynamoDBClient = dynamoDBClient;
            _entityTypes = Assembly.Load("DataLoadTool.Core")
                .GetTypes()
                .Where(t => t.Namespace == "DataLoadTool.Core.Entities" &&
                            t.GetCustomAttribute<DynamoDBTableAttribute>() != null)
                .ToList();

            _defaultPrefix = configuration["AWS:DynamoDB:TablePrefix"];
        }

        private async Task<bool> TableExistsAsync(string tableName)
        {
            var tables = await _dynamoDBClient.ListTablesAsync();
            return tables.TableNames.Contains(tableName);
        }

        public async Task RemoveAsync()
        {
            foreach (var entityType in _entityTypes)
            {
                var tableName = _defaultPrefix + entityType.GetCustomAttribute<DynamoDBTableAttribute>().TableName;
                var tableExists = await TableExistsAsync(tableName);
                if (tableExists)
                {
                    await _dynamoDBClient.DeleteTableAsync(tableName);
                }
            }
        }

        public async Task MigrateAsync()
        {
            foreach (var entityType in _entityTypes)
            {
                var tableName = _defaultPrefix + entityType.GetCustomAttribute<DynamoDBTableAttribute>().TableName;
                var tableExists = await TableExistsAsync(tableName);
                if (!tableExists)
                {
                    var createTableRequest = CreateTableRequestForEntity(entityType);
                    await _dynamoDBClient.CreateTableAsync(createTableRequest);
                }
            }
        }

        private CreateTableRequest CreateTableRequestForEntity(Type entityType)
        {
            var entityName = entityType.GetCustomAttribute<DynamoDBTableAttribute>().TableName;
            var tableName = _defaultPrefix + entityName;
            var keySchema = new List<KeySchemaElement>();
            var attributeDefinitions = new List<AttributeDefinition>();

            foreach (var property in entityType.GetProperties())
            {
                var attributeName = property.GetCustomAttribute<DynamoDBPropertyAttribute>()?.AttributeName ?? property.Name.ToLower();

                if (property.GetCustomAttribute<DynamoDBHashKeyAttribute>() != null)
                {
                    keySchema.Add(new KeySchemaElement(attributeName, KeyType.HASH));
                    attributeDefinitions.Add(new AttributeDefinition(attributeName, GetScalarAttributeType(property.PropertyType)));
                }
                else if (property.GetCustomAttribute<DynamoDBRangeKeyAttribute>() != null)
                {
                    keySchema.Add(new KeySchemaElement(attributeName, KeyType.RANGE));
                    attributeDefinitions.Add(new AttributeDefinition(attributeName, GetScalarAttributeType(property.PropertyType)));
                }
            }

            var tableRequest = new CreateTableRequest
            {
                TableName = tableName,
                KeySchema = keySchema,
                AttributeDefinitions = attributeDefinitions,
                BillingMode = BillingMode.PAY_PER_REQUEST
            };

            if (entityName == "user")
            {
                string emailAttributeName = "email";

                if (!tableRequest.AttributeDefinitions.Any(ad => ad.AttributeName == emailAttributeName))
                {
                    tableRequest.AttributeDefinitions.Add(new AttributeDefinition(emailAttributeName, ScalarAttributeType.S));
                }

                var gsi = new GlobalSecondaryIndex
                {
                    IndexName = $"{tableName}_email",
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement(emailAttributeName, KeyType.HASH)
                    },
                    Projection = new Projection
                    {
                        ProjectionType = ProjectionType.ALL // Adjust projection type as needed
                    }
                };

                tableRequest.GlobalSecondaryIndexes = new List<GlobalSecondaryIndex> { gsi };
            }

            return tableRequest;
        }

        // https://blog.awsfundamentals.com/aws-dynamodb-data-types
        private ScalarAttributeType GetScalarAttributeType(Type type)
        {
            if (type == typeof(string) || type == typeof(DateTime))
                return ScalarAttributeType.S;
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
                return ScalarAttributeType.N;
            if (type == typeof(byte[]))
                return ScalarAttributeType.B;
            throw new Exception("Unsupported attribute type");
        }
    }
}
