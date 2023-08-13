using MergeArchiveFile.Dto;
using MergeArchiveFile.Services.Interface;
using MergeArchiveFile.UtilityHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MergeArchiveFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MergeArchivesController : ControllerBase
    {
        private readonly IMergeFilesService _mergeFilesService;
        private readonly ILogger<MergeArchivesController> _logger;
        private readonly FileUploadSettings _fileUploadSettings;
        public MergeArchivesController(IMergeFilesService mergeFilesService, IOptions<FileUploadSettings> fileUploadSettings, ILogger<MergeArchivesController> logger)
        {
            _mergeFilesService = mergeFilesService;
            _logger = logger;
            _fileUploadSettings = fileUploadSettings.Value;
        }

        /// <summary>
        /// To upload and merge multiple files, then return the merged file as a ZIP archive
        /// </summary>
        /// <param name="files">The list of zip files for merging.</param>
        /// <response code="200">The merged zip file.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpPost]
        [Produces("application/zip")]  
        [Consumes("multipart/form-data")] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromForm] List<IFormFile> files)
        {
            List<string> errorMessages = _mergeFilesService.ValidateFiles(files, _fileUploadSettings);
            if (errorMessages.Any())
            {
                _logger.LogError(string.Join(",", errorMessages.ToArray()));
                return BadRequest(errorMessages);
            }

            var mergedFile = await _mergeFilesService.MergeFilesAsync(files);
            return File(mergedFile, "application/zip", $"Merged_Archive_Files_{Guid.NewGuid()}.zip");
        }


        [HttpPost("mergezipfileandsaveondisc")]
        [Produces("application/zip")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MergeZipFilesAndSave([FromForm] List<IFormFile> files)
        {
            List<string> errorMessages = _mergeFilesService.ValidateFiles(files, _fileUploadSettings);
            if (errorMessages.Any())
            {
                _logger.LogError(string.Join(",", errorMessages.ToArray()));
                return BadRequest(errorMessages);
            }

            var mergedFile = await _mergeFilesService.MergeFilesAndSaveOnDiscAsync(files);
            return File(mergedFile, "application/zip", $"Merged_Archive_Files_{Guid.NewGuid()}.zip");
        }

    }
}
