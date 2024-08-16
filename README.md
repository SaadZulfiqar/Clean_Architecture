# ADL - Data Load Tool Web API

## Overview

Data Load Tool Web API - BoilderPlate

## Features

- **Batch File Upload**: Upload multiple files at once.
- **Validation**: Make sure uploaded files match a specific template.
- **Transformation**: Convert data into a format supported by Aria.
- **Data Upload**: Upload the transformed data using Aria's APIs.

## Technologies Used

- **.NET Core Version**: 6
- **Database**: DynamoDB
- **AWS Services**:
  - **S3**: For storing uploaded files

## Clean Architecture Project Structure

- **Core**: Contains domain models and interfaces.
- **Application**: Contains business logic and use cases.
- **Infrastructure**: Handles external services like database and file storage.
- **API**: Manages HTTP requests and responses.

## Getting Started

### Setting Up the Development/Local Environment

#### Configure Environment-Specific Settings

1. **Create Configuration Files**:
   - `appsettings.json`: Base configuration file.
   - `appsettings.Development.json`: Settings for local development.
   - `appsettings.UAT.json`: Settings for UAT.
   - `appsettings.Production.json`: Settings for production.

2. **Set the Environment**:
   - Use the `ASPNETCORE_ENVIRONMENT` variable to set the environment (e.g., Development/Local, UAT, Production).

3. **Load Configuration in `Program.cs`**:
   - Ensure the appropriate configuration files are loaded based on the environment setting.

#### Setting Up Environment Variables

- **In Visual Studio**:
  - Right-click your project in Solution Explorer, select Properties, then go to the Debug tab. Set `ASPNETCORE_ENVIRONMENT` in the Environment Variables section.

- **In the Command Line**:
  - **Windows**: `set ASPNETCORE_ENVIRONMENT=Development`
  - **Linux/macOS**: `export ASPNETCORE_ENVIRONMENT=Development`

#### Summary

- `appsettings.json`: Base settings file.
- `appsettings.{Environment}.json`: Overrides for specific environments.
- `ASPNETCORE_ENVIRONMENT`: Specifies the current environment.
- `Program.cs`: Loads settings based on the environment.

### Setting Up DynamoDB Locally

1. **Download DynamoDB Local**:
   - Go to [DynamoDB Local Documentation](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.DownloadingAndRunning.html) and download the latest version.

2. **Prerequisites**:
   - Install Java Runtime and AWS CLI.

3. **Run DynamoDB Local**:
   - Navigate to the folder containing `dynamodb_local_latest` and run:
     ```
     java -Djava.library.path=./DynamoDBLocal_lib -jar DynamoDBLocal.jar -sharedDb
     ```

4. **Set Up AWS CLI with Fake Credentials**:
   - Run `aws configure` and use placeholder values:
     ```
     AWS Access Key ID [None]: x
     AWS Secret Access Key [None]: x
     Default region name [None]: x
     Default output format [None]: json
     ```

5. **Check Setup**:
   - Use this command to see if DynamoDB is running:
     ```
     aws dynamodb list-tables --endpoint-url http://localhost:8000
     ```
	 
   - Use this command to see schema of specific DynamoDB table, for instance 'user':
     ```
     aws dynamodb describe-table --table-name user --endpoint-url http://localhost:8000
     ```
	 
   - Other commands like scan using the cli:
     ```
	 aws dynamodb scan --table-name dev_user --endpoint-url http://localhost:8000
     ```
	 
   - Use following to list down the commands available:
     ```
	 aws dynamodb help
     ```
	 
### Prerequisites

- [.NET Core 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- AWS account with access to S3 and DynamoDB
