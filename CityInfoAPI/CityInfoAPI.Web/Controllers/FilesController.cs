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
    }
}
