namespace DataLoadTool.Application.Utilities
{
    public static class UniqueGuidGenerator
    {
        public static string GenerateUniqueGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
