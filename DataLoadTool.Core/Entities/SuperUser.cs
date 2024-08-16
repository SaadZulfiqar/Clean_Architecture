using Amazon.DynamoDBv2.DataModel;

namespace DataLoadTool.Core.Entities
{
    /// <summary>
    /// Rules: 
    /// 1. All super users must have unique Email
    /// 
    /// Type: Simple Primary Key (Partition Key Only)
    /// PartitionKey: Id
    /// SortKey: N/A
    /// </summary>

    [DynamoDBTable("super_user")]
    public class SuperUser
    {
        [DynamoDBHashKey("id")]
        public string Id { get; set; }

        [DynamoDBProperty("email")]
        public string Email { get; set; }

        [DynamoDBProperty("status")]
        public bool Status { get; set; }

        [DynamoDBProperty("hash")]
        public string Hash { get; set; }

        [DynamoDBProperty("salt")]
        public string Salt { get; set; }

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
