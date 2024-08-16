using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DataLoadTool.Core.Interfaces;
using System.Reflection;

namespace DataLoadTool.Infrastructure.Persistence
{
    // https://www.rahulpnath.com/blog/dynamodb-querying-dotnet/
    public class DynamoDBContextWithPrefix : DynamoDBContext, IDynamoDBContextWithPrefix
    {
        private readonly string _defaultPrefix;
        public DynamoDBContextWithPrefix(IAmazonDynamoDB client, string defaultPrefix) : base(client)
        {
            _defaultPrefix = defaultPrefix ?? string.Empty;
        }
        private string GetPrefixedTableName(Type type)
        {
            var tableAttr = type.GetCustomAttribute<DynamoDBTableAttribute>();
            return _defaultPrefix + tableAttr?.TableName;
        }
        public Task SaveAsync<T>(T item, DynamoDBOperationConfig operationConfig = null)
        {
            operationConfig = operationConfig ?? new DynamoDBOperationConfig();
            operationConfig.OverrideTableName = GetPrefixedTableName(typeof(T));
            return base.SaveAsync(item, operationConfig);
        }
        public async Task<IEnumerable<T>> ScanAsync<T>(IEnumerable<ScanCondition> conditions, DynamoDBOperationConfig operationConfig = null)
        {
            operationConfig = operationConfig ?? new DynamoDBOperationConfig();
            operationConfig.OverrideTableName = GetPrefixedTableName(typeof(T));
            return await base.ScanAsync<T>(conditions, operationConfig).GetRemainingAsync();
        }
        public Task<T> LoadItemAsync<T>(object hashKey, object rangeKey = null, DynamoDBOperationConfig operationConfig = null)
        {
            operationConfig = operationConfig ?? new DynamoDBOperationConfig();
            operationConfig.OverrideTableName = GetPrefixedTableName(typeof(T));
            return base.LoadAsync<T>(hashKey, rangeKey, operationConfig);
        }
        public async Task<IEnumerable<T>> QueryAsyncByIndex<T>(object hashKey, string indexName, List<ScanCondition> scanConditions = null, DynamoDBOperationConfig operationConfig = null)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                throw new ArgumentException("Index name is required.");
            }

            operationConfig = operationConfig ?? new DynamoDBOperationConfig();
            operationConfig.OverrideTableName = GetPrefixedTableName(typeof(T));
            operationConfig.IndexName = $"{GetPrefixedTableName(typeof(T))}{indexName}";

            if (scanConditions != null)
            {
                operationConfig.QueryFilter = scanConditions;
            }

            return await base.QueryAsync<T>(hashKey, operationConfig).GetRemainingAsync();
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string hashKey, DynamoDBOperationConfig operationConfig = null)
        {
            operationConfig = operationConfig ?? new DynamoDBOperationConfig();
            operationConfig.OverrideTableName = GetPrefixedTableName(typeof(T));

            var asyncSearch = base.QueryAsync<T>(hashKey, operationConfig);
            return await asyncSearch.GetRemainingAsync();
        }

        public async Task DeleteAsync<T>(string hashKey, DynamoDBOperationConfig operationConfig = null)
        {
            operationConfig = operationConfig ?? new DynamoDBOperationConfig();
            operationConfig.OverrideTableName = GetPrefixedTableName(typeof(T));

            await base.DeleteAsync<T>(hashKey, operationConfig);
        }
    }
}
