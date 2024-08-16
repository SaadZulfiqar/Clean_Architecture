using Amazon.DynamoDBv2.DataModel;
using System.Threading.Tasks;

namespace DataLoadTool.Core.Interfaces
{
    public interface IDynamoDBContextWithPrefix
    {
        Task SaveAsync<T>(T item, DynamoDBOperationConfig operationConfig = null);
        Task<T> LoadItemAsync<T>(object hashKey, object rangeKey = null, DynamoDBOperationConfig operationConfig = null);
        Task<IEnumerable<T>> QueryAsyncByIndex<T>(object hashKey, string indexName, List<ScanCondition> scanConditions = null, DynamoDBOperationConfig operationConfig = null);
        Task<IEnumerable<T>> ScanAsync<T>(IEnumerable<ScanCondition> conditions, DynamoDBOperationConfig operationConfig = null);
        Task<IEnumerable<T>> QueryAsync<T>(string hashKey, DynamoDBOperationConfig operationConfig = null);
        Task DeleteAsync<T>(string hashKey, DynamoDBOperationConfig operationConfig = null);
    }
}
