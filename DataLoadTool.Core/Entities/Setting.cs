using Amazon.DynamoDBv2.DataModel;

namespace DataLoadTool.Core.Entities
{
    /// <summary>
    /// Rules: 
    /// 1. All settings must have unique Id, within a tenant
    /// 
    /// Type: Composite Primary Key (Partition Key + Sort Key)
    /// PartitionKey: Tenant_Id
    /// SortKey: Id
    /// </summary>

    [DynamoDBTable("setting")]
    public class Setting
    {
        [DynamoDBHashKey("tenant_id")]
        public string Tenant_Id { get; set; }

        [DynamoDBRangeKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("configuration")]
        public List<string> Configuration { get; set; } // Array column

        [DynamoDBProperty("created_by")]
        public string CreatedBy { get; set; }

        [DynamoDBProperty("updated_by")]
        public string UpdatedBy { get; set; }

        [DynamoDBProperty("created_date")]
        public DateTime CreatedDate { get; set; }

        [DynamoDBProperty("updated_date")]
        public DateTime UpdatedDate { get; set; }
    }
}
