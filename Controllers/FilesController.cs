using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace DemoApp.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        /*
         *  Setting up Content Type Provider for getting right content type for download
         */
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
                ?? throw new System.ArgumentNullException (
                    nameof(fileExtensionContentTypeProvider));
        }     
        

        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            /*
            *   Look up the actual file, depending on the fileId          
            */
            var pathToFile = "159198.gif";

            /*
             *  Check whether the file exists
             */
            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound();
            }

            /*
             *  Try to get right content type for a file
             */
            if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(pathToFile);
            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }
    }
}
