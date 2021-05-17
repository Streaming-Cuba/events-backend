using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Events.API.Models;
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
        [HttpPost("folder/{*path}")]
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

        [Authorize(Roles = "Administrator")]
        [HttpDelete("folder/{*path}")]
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

        [Authorize(Roles = "Administrator")]
        [HttpPost("file/{*path}")]
        public async Task<IActionResult> CreateFile([FromRoute] string path, [FromQuery] bool overwrite, [Required] IFormFile file)
        {
            // FIX: Added filter to file from request
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

        [HttpGet("file/{*path}")]
        public IActionResult DownloadFile([FromRoute] string path)
                => RedirectPermanent($"https://media.streamingcuba.com/{path}");

        [Authorize(Roles = "Administrator")]
        [HttpGet("folder/{*path}")]
        public ActionResult<List<EntryInfo>> EnumerateEntries([FromRoute] string path)
        {
            string directoryPath = null;
            if (path != null)
            {
                if (Path.GetInvalidPathChars().Any(x => path.Contains(x)))
                    return BadRequest(new
                    {
                        error = "The path contains invalid characters"
                    });

                directoryPath = Path.Join(_basePath, path);
                if (!Directory.Exists(directoryPath))
                    return BadRequest(new
                    {
                        error = "This folder not exists"
                    });
            }
            else
            {
                directoryPath = _basePath;
            }

            var directoryInfo = new DirectoryInfo(directoryPath);
            List<EntryInfo> result = directoryInfo.EnumerateFileSystemInfos().Select(x => new EntryInfo
            {
                Name = x.Name,
                Extension = x.Extension,
                ModificationTime = x.LastWriteTimeUtc,
                IsDir = x.Attributes.HasFlag(FileAttributes.Directory),
                Size = x.Attributes.HasFlag(FileAttributes.Directory) ? 0 : ComputeFileSize(x.FullName)
            }).ToList();

            return Ok(result);
        }

        private long ComputeFileSize(string path) => (new FileInfo(path)).Length;

        private long ComputeFolderSize(string path) 
            => Directory.GetFiles(path).Select(x => (new FileInfo(path)).Length).Sum() +
                Directory.GetDirectories(path).Select(x => ComputeFolderSize(x)).Sum();
    }
}
