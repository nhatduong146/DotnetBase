using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using DotnetBase.Media.Common.Helpers;
using Microsoft.Extensions.Logging;

namespace DotnetBase.Media.Controllers
{
    [Route("api/media")]
    public class MediaController : Controller
    {
        private readonly ILogger<MediaController> _logger;

        public MediaController(ILogger<MediaController> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(string folder, string fileName, IFormFile file)
        {
            _logger.LogInformation($"Uploading the {fileName} to {folder}");
            var t1 = Task.Run(() => FileHelper.SaveFile(folder, fileName, file));

            await Task.WhenAll(t1);

            var extension = Path.GetExtension(file.FileName);

            return Ok(Url.Action("GetFile", "Media", new { folder = folder, fileName = string.Format("{0}{1}", fileName, extension) }, Request.Scheme));
        }

        [HttpGet("{folder}/{fileName}")]
        public async Task<IActionResult> GetFile(string folder, string fileName)
        {
            var t1 = Task.Run(() => FileHelper.GetFile(folder, fileName));

            await Task.WhenAll(t1);

            var fileStream = System.IO.File.OpenRead(t1.Result);

            var contentType = GetMimeType(fileName);

            return base.File(fileStream, contentType);
        }

        private string GetMimeType(string fileName)
        {
            return MimeTypes.GetMimeType(fileName);
        }
    }
}