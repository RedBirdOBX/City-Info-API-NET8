using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfoAPI.Web.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/files")]
    [Authorize]
    [ApiVersion("2.0")]
    public class FilesController : Controller
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        private readonly ILogger<FilesController> _logger;

        /// <summary>constructor</summary>
        /// <param name="fileExtensionContentTypeProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider, ILogger<FilesController> logger)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>retrieves file by id</summary>
        /// <param name="fileId"></param>
        /// <returns>file found by id</returns>
        /// <example>{baseUrl}/api/files/{fileId}</example>
        [HttpGet("{fileId}", Name = "GetFile")]
        public ActionResult GetFile([FromRoute] string fileId)
        {
            try
            {
                // choices:
                //  FileContentResult
                //  FileStreamResult
                //  PhysicalFileResult
                //  VirtualFileResult

                string file = string.Empty;
                switch (fileId)
                {
                    case "1":
                        file = "Downloads\\Test.txt";
                        break;
                    case "2":
                        file = "Downloads\\chicken-recipe.pdf";
                        break;
                    case "3":
                        file = "Downloads\\shrimp-recipe.pdf";
                        break;
                    default:
                        file = "Downloads\\shrimp-recipe.pdf";
                        break;
                }

                if (!System.IO.File.Exists(file))
                {
                    _logger.LogWarning($"file {file} not found.");
                    return NotFound($"file {file} not found.");
                }

                if (!_fileExtensionContentTypeProvider.TryGetContentType(file, out var contentType))
                {
                    // default media type for binary data
                    contentType = "application/octet-stream";
                }

                var bytes = System.IO.File.ReadAllBytes(file);
                return File(bytes, contentType, Path.GetFileName(file));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting file.");
                return StatusCode(500, "An error occurred while getting file.");
            }
        }

        /// <summary>Deprecated Example</summary>
        /// <param name="fileId"></param>
        /// <returns>file found by id</returns>
        /// <example>{baseUrl}/api/files/{fileId}</example>
        //[HttpGet("legacy/{fileId}", Name = "GetFileLegacy")]
        //[ApiVersion(0.1, Deprecated = true)]
        //public ActionResult GetFileLegacy([FromRoute] string fileId)
        //{
        //    string file = "Downloads\\Test.txt";
        //    var bytes = System.IO.File.ReadAllBytes(file);
        //    return File(bytes, "application/text", Path.GetFileName(file));
        //}

        /// <summary>create a pdf file</summary>
        /// <param name="file"></param>
        /// <returns>OK - 200</returns>
        /// <example>{baseUrl}/api/files</example>
        [HttpPost]
        public async Task<ActionResult> CreateFile(IFormFile file)
        {
            try
            {
                // validate input
                if (file == null)
                {
                    return BadRequest("No file detected.");
                }

                if (file.Length == 0 || file.Length > 20971520 || file.ContentType != "application/pdf")
                {
                    return BadRequest("Invalid file.");
                }

                // ideally, you would store files on a separate disk which does not have execute permissions.
                // or, perhaps upload to a cloud storage location. finally, it is better NOT to use the name of the file uploaded by the user.
                var path = Path.Combine(Directory.GetCurrentDirectory(), $"uploads/uploaded_file_{Guid.NewGuid()}.pdf");

                using (var steam = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(steam);
                }

                return Ok("Your file has been uploaded.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating file.");
                return StatusCode(500, "An error occurred while creating file.");
            }
        }
    }
}
