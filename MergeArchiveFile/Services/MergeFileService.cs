using MergeArchiveFile.Services.Interface;
using MergeArchiveFile.UtilityHelpers;
using System.IO;
using System.IO.Compression;
using System.Reflection.Metadata;

namespace MergeArchiveFile.Services
{
    public class MergeFilesService : IMergeFilesService
    {
        public async Task<MemoryStream> MergeFilesAsync(List<IFormFile> files)
        {
            var outputMemStream = new MemoryStream();

            using (var archive = new ZipArchive(outputMemStream, ZipArchiveMode.Create, true))
            {
                foreach (var formFile in files)
                {
                    using (var inputMemStream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(inputMemStream);
                        inputMemStream.Position = 0;
                        using (var inputArchive = new ZipArchive(inputMemStream, ZipArchiveMode.Read, true))
                        {
                            foreach (var entry in inputArchive.Entries)
                            {
                                var uniqueEntryName = $"{Guid.NewGuid()}_{entry.Name}";
                                var newEntry = archive.CreateEntry(uniqueEntryName);

                                using (var entryStream = entry.Open())
                                using (var newEntryStream = newEntry.Open())
                                {
                                    await entryStream.CopyToAsync(newEntryStream);
                                }
                            }
                        }
                    }
                }
            }

            outputMemStream.Position = 0;

            return outputMemStream;
        }
        public async Task<byte[]> MergeFilesAndSaveOnDiscAsync(List<IFormFile> files)
        {
            string tempOutputPath = Path.GetTempFileName();

            using (var fileStream = new FileStream(tempOutputPath, FileMode.Create))
            using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
            {
                foreach (var formFile in files)
                {
                    string tempInputPath = Path.GetTempFileName();
                    using (var tempStream = new FileStream(tempInputPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(tempStream);
                    }

                    using (var tempStream = new FileStream(tempInputPath, FileMode.Open))
                    using (var inputArchive = new ZipArchive(tempStream, ZipArchiveMode.Read, true))
                    {
                        foreach (var entry in inputArchive.Entries)
                        {
                            var uniqueEntryName = $"{Guid.NewGuid()}_{entry.Name}";
                            var newEntry = archive.CreateEntry(uniqueEntryName);

                            using (var entryStream = entry.Open())
                            using (var newEntryStream = newEntry.Open())
                            {
                                await entryStream.CopyToAsync(newEntryStream);
                            }
                        }
                    }

                    File.Delete(tempInputPath);
                }
            }

            return await File.ReadAllBytesAsync(tempOutputPath);
        }
     
        public List<string> ValidateFiles(List<IFormFile> file, FileUploadSettings fileSettings)
        {
            List<string> errorMessages = new();
            if (file == null || file.Count == 0)
                errorMessages.Add("No files uploaded");
            else
            {
                foreach (var formFile in file)
                {
                    if (!IsValidFile(formFile, fileSettings))
                        errorMessages.Add($"Uploaded file with name: {formFile.FileName} is invalid");
                }
            }
            return errorMessages;
        }

        private bool IsValidFile(IFormFile file, FileUploadSettings fileSettings)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return file.Length <= fileSettings.MaximumAllowedContentLength && fileSettings.AllowedFileExtensions.Contains(extension);
        }

    }
}
