using Amazon.DynamoDBv2.DataModel;

namespace DataLoadTool.Core.Entities
{
    /// <summary>
    /// Rules: 
    /// 1. All users must have unique email within a tenant
    /// 2. Id must be unique
    /// 3. GSI will also be created (PK: email, SK: tenant_id, IndexName: <db_table_prefix>_email)
    /// 
    /// Type: Composite Primary Key (Partition Key + Sort Key)
    /// PartitionKey: Tenant_Id
    /// SortKey: Id
    /// </summary>

    [DynamoDBTable("user")]
    public class User
    {
        [DynamoDBHashKey("tenant_id")]
        public string Tenant_Id { get; set; }

        [DynamoDBRangeKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("email")]
        public string Email { get; set; }

        [DynamoDBProperty("hash")]
        public string Hash { get; set; }

        [DynamoDBProperty("salt")]
        public string Salt { get; set; }

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
