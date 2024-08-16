using Amazon.DynamoDBv2.DataModel;

namespace DataLoadTool.Core.Entities
{
    /// <summary>
    /// Rules: 
    /// 1. All roles must have unique Id, within a tenant
    /// 
    /// Type: Composite Primary Key (Partition Key + Sort Key)
    /// PartitionKey: Tenant_Id
    /// SortKey: Id
    /// </summary>

    [DynamoDBTable("role")]
    public class Role
    {
        [DynamoDBHashKey("tenant_id")]
        public string Tenant_Id { get; set; }

        [DynamoDBRangeKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("name")]
        public string Name { get; set; }

        [DynamoDBProperty("description")]
        public string Description { get; set; }

        [DynamoDBProperty("status")]
        public bool Status { get; set; }

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
