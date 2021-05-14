using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Events.API.Services.CDN;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Events.API.Controllers
{
    [Route("/api/v1/media")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly string _basePath;

        public MediaController(ICdnService cdnService, IConfiguration configuration)
        {
            _cdnservice = cdnService;
            _basePath = configuration["Media:Path"];
        }

        public readonly ICdnService _cdnservice;

        [HttpPost("upload")]
        public IActionResult UploadPhoto(IFormFile photo)
        {
            var stream = photo.OpenReadStream();
            var result = _cdnservice.UploadPhoto(photo.FileName, stream);

            return Ok(new
            {
                Url = result.Url
            });
        }

        [HttpPost("folder/{path}")]
        public IActionResult CreateFolder([FromRoute] string path)
        {
            if (Path.GetInvalidPathChars().Any(x => path.Contains(x)))
                return BadRequest(new
                {
                    error = "The path contains invalid characters"
                });
            
            string directoryPath = Path.Join(_basePath, path);
            if (Directory.Exists(directoryPath))
                return BadRequest(new
                {
                    error = "This folder already exists"
                });
            Directory.CreateDirectory(directoryPath);
            return Ok();
        }

        [HttpDelete("folder/{path}")]
        public IActionResult DeleteFolder([FromRoute] string path)
        {
            if (Path.GetInvalidPathChars().Any(x => path.Contains(x)))
                return BadRequest(new
                {
                    error = "The path contains invalid characters"
                });
            
            string directoryPath = Path.Join(_basePath, path);
            if (!Directory.Exists(directoryPath))
                return BadRequest(new
                {
                    error = "This folder not exists"
                });

            Directory.Delete(directoryPath, true);
            return Ok();
        }
    }
}
