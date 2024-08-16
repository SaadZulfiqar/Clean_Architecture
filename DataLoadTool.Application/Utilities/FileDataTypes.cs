namespace DataLoadTool.Application.Utilities
{
    public enum FileDataTypes
    {
        PRODUCT,
        CUSTOMER
    }

    // Create an extension method
    public static class FileDataTypesExtensions
    {
        public static string ToFriendlyString(this FileDataTypes fileType)
        {
            return fileType switch
            {
                FileDataTypes.PRODUCT => "Product",
                FileDataTypes.CUSTOMER => "Customer",
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }
    }
}
