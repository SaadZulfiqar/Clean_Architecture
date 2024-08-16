using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace DataLoadTool.Infrastructure.Persistence
{
    public class DynamoDbContextOld
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly DynamoDBContext _context;

        public DynamoDbContextOld(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
            _context = new DynamoDBContext(_dynamoDbClient);
        }

        // Method to save an entity
        public async Task SaveAsync<T>(T entity) where T : class
        {
            await _context.SaveAsync(entity);
        }

        // Method to load an entity by partition key
        public async Task<T> LoadAsync<T>(object partitionKey) where T : class
        {
            return await _context.LoadAsync<T>(partitionKey);
        }

        // Method to load an entity by partition and sort key
        public async Task<T> LoadAsync<T>(object partitionKey, object sortKey) where T : class
        {
            return await _context.LoadAsync<T>(partitionKey, sortKey);
        }

        // Method to delete an entity by partition key
        public async Task DeleteAsync<T>(object partitionKey) where T : class
        {
            await _context.DeleteAsync<T>(partitionKey);
        }

        // Method to delete an entity by partition and sort key
        public async Task DeleteAsync<T>(object partitionKey, object sortKey) where T : class
        {
            await _context.DeleteAsync<T>(partitionKey, sortKey);
        }

        // Method to query entities by a specified condition
        public async Task<IEnumerable<T>> QueryAsync<T>(string indexName, object hashKey, IEnumerable<ScanCondition> conditions) where T : class
        {
            var queryConfig = new DynamoDBOperationConfig
            {
                IndexName = indexName,
                QueryFilter = new List<ScanCondition>(conditions)
            };

            return await _context.QueryAsync<T>(hashKey, queryConfig).GetRemainingAsync();
        }

        // Method to scan entities with specific conditions
        public async Task<IEnumerable<T>> ScanAsync<T>(IEnumerable<ScanCondition> conditions) where T : class
        {
            return await _context.ScanAsync<T>(conditions).GetRemainingAsync();
        }
    }
}
