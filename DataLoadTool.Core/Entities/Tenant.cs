using Amazon.DynamoDBv2.DataModel;

namespace DataLoadTool.Core.Entities
{
    /// <summary>
    /// Rules: 
    /// 1. All tenants must have a unique ID.
    /// 2. The name must be unique as well.
    ///
    /// Type: Simple Primary Key (Partition Key Only)
    /// PartitionKey: Id
    /// SortKey: N/A
    /// </summary>

    [DynamoDBTable("tenant")]
    public class Tenant
    {
        [DynamoDBHashKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("name")]
        public string Name { get; set; }

        [DynamoDBProperty("description")]
        public string Description { get; set; }

        [DynamoDBProperty("status")]
        public bool Status { get; set; }

        [DynamoDBProperty("created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty("created_date")]
        public DateTime CreatedDate { get; set; }

        [DynamoDBProperty("updated_by")]
        public string UpdatedBy { get; set; }

        [DynamoDBProperty("updated_date")]
        public DateTime UpdatedDate { get; set; }
    }
}
