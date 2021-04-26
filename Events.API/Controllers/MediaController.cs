using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Events.API.Services.CDN;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [Route("/api/v1/media")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        public MediaController(ICdnService cdnService)
        {
            _cdnservice = cdnService;
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
    }
}
