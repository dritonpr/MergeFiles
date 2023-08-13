using MergeArchiveFile.Dto;
using MergeArchiveFile.UtilityHelpers;
using Microsoft.Extensions.Options;

namespace MergeArchiveFile.Services.Interface
{
    public interface IMergeFilesService
    {
        Task<MemoryStream> MergeFilesAsync(List<IFormFile> files);
        List<string> ValidateFiles(List<IFormFile> files, FileUploadSettings fileUploadSettings);
        Task<byte[]> MergeFilesAndSaveOnDiscAsync(List<IFormFile> files);

    }
}
