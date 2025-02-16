using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfoAPI.Web.Controllers
{
    [Route("api/files")]
    public class FilesController : Controller
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="fileExtensionContentTypeProvider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider, ILogger<FilesController> logger)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("{fileId}", Name = "GetFile")]
        public ActionResult GetFile([FromRoute] string fileId)
        {
            // choices..
            // FileContentResult
            // FileStreamResult
            // PhysicalFileResult
            // VirtualFileResult

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateFile(IFormFile file)
        {
            // validate input
            if (file.Length == 0 || file.Length > 20971520 || file.ContentType != "application/pdf")
            {
                return BadRequest("Invalid file.");
            }

            // ideally, you would store files on a separate disk which does not have execute permissions
            // is also better NOT to use the name of the file uploaded by the user.
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded_file_{Guid.NewGuid()}.pdf");

            using (var steam = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(steam);
            }

            return Ok("Your file has been uploaded.");
        }
    }
}
