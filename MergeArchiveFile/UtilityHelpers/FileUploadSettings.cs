namespace MergeArchiveFile.UtilityHelpers
{
    public class FileUploadSettings
    {
        public List<string> AllowedFileExtensions { get; set; }
        public long MaximumAllowedContentLength { get; set; }
        public int MaximumFileSize { get; set; }
    }
}
