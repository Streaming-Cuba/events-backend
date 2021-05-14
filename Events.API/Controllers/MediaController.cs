using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public MediaController(IConfiguration configuration)
        {
            _basePath = configuration["Media:Path"];
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("folder/{path}")]
        public IActionResult CreateFolder([FromRoute] string path)
        {
            path = Uri.UnescapeDataString(path);
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

        [Authorize(Roles = "Administrator")]
        [HttpDelete("folder/{path}")]
        public IActionResult DeleteFolder([FromRoute] string path)
        {
            path = Uri.UnescapeDataString(path);
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

        [HttpPost("file/{path}")]
        public async Task<IActionResult> CreateFile([FromRoute] string path, [FromQuery] bool overwrite, IFormFile file)
        {
            path = Uri.UnescapeDataString(path);
            if (Path.GetInvalidPathChars().Any(x => path.Contains(x)))
                return BadRequest(new
                {
                    error = "The path contains invalid characters"
                });

            string filePath = Path.Join(_basePath, path);
            if (!overwrite && System.IO.File.Exists(filePath))
                return BadRequest(new
                {
                    error = "This file already exists"
                });

            try
            {
                using (var stream = System.IO.File.OpenWrite(filePath))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (PathTooLongException)
            {
                return BadRequest(new 
                {
                    error = "The result path is too long"
                });
            }

            return Ok();
        }
    }
}
