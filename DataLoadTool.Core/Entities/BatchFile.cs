using Amazon.DynamoDBv2.DataModel;

namespace DataLoadTool.Core.Entities
{
    /// <summary>
    /// Rules: 
    /// 1. All batch files must have unique Id, within a batch, and batch is unique within a tenant
    /// 2. In order to see, what are the files, that are part of the tenant, create a global secondary index (PK: Tenant_Id, and SK: Batch_Id/CreatedDate), but do we need it? 
    /// 
    /// Type: Composite Primary Key (Partition Key + Sort Key)
    /// PartitionKey: Batch_Id
    /// SortKey: Id
    /// </summary>

    [DynamoDBTable("batch_file")]
    public class BatchFile
    {
        [DynamoDBHashKey("batch_Id")]
        public string Batch_Id { get; set; }

        [DynamoDBRangeKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("tenant_id")]
        public string Tenant_Id { get; set; }

        [DynamoDBProperty("file_path")]
        public string FilePath { get; set; }

        [DynamoDBProperty("status")]
        public string Status { get; set; }

        [DynamoDBProperty("details")]
        public string Details { get; set; } = string.Empty;

        [DynamoDBProperty("created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty("created_date")]
        public DateTime CreatedDate { get; set; }
    }
}