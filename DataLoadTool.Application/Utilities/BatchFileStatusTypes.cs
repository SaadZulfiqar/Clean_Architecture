namespace DataLoadTool.Application.Utilities
{
    public enum BatchFileStatusTypes
    {
        UploadInProgress,
        UploadCompleted,
        UploadFailed,
        ValidationInProgress,
        ValidationCompleted,
        ValidationFailed,
        TransformationInProgress,
        TransformationCompleted,
        TransformationFailed,
        PushInProgress,
        PushCompleted,
        PushFailed
    }

    // Create an extension method
    public static class BatchFileStatusTypesExtensions
    {
        public static string ToFriendlyString(this BatchFileStatusTypes statusType)
        {
            return statusType switch
            {
                BatchFileStatusTypes.UploadInProgress => "Upload In Progress",
                BatchFileStatusTypes.UploadCompleted => "Upload Completed",
                BatchFileStatusTypes.UploadFailed => "Upload Failed",
                BatchFileStatusTypes.ValidationInProgress => "Validation In Progress",
                BatchFileStatusTypes.ValidationCompleted => "Validation Completed",
                BatchFileStatusTypes.ValidationFailed => "Validation Failed",
                BatchFileStatusTypes.TransformationInProgress => "Transformation In Progress",
                BatchFileStatusTypes.TransformationCompleted => "Transformation Completed",
                BatchFileStatusTypes.TransformationFailed => "Transformation Failed",
                BatchFileStatusTypes.PushInProgress => "Push In Progress",
                BatchFileStatusTypes.PushCompleted => "Push Completed",
                BatchFileStatusTypes.PushFailed => "Push Failed",
                _ => throw new ArgumentOutOfRangeException(nameof(statusType), statusType, null)
            };
        }
    }
}
