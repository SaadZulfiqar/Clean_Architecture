using Amazon.DynamoDBv2.DataModel;

namespace DataLoadTool.Core.Entities
{
    /// <summary>
    /// Rules: 
    /// 1. All batches must have unique Id, within a tenant
    /// 
    /// Type: Composite Primary Key (Partition Key + Sort Key)
    /// PartitionKey: Tenant_Id
    /// SortKey: Id
    /// </summary>

    [DynamoDBTable("batch")]
    public class Batch
    {
        [DynamoDBHashKey("tenant_id")]
        public string Tenant_Id { get; set; }

        [DynamoDBRangeKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty("created_date")]
        public DateTime CreatedDate { get; set; }

        [DynamoDBProperty("files")]
        public List<string> Files { get; set; } = new List<string>();
    }
}
