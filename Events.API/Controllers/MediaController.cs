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
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        public MediaController(ICdnService cdnService)
        {
            CdnService = cdnService;
        }

        public ICdnService CdnService { get; }

        [HttpPost("upload")]
        public IActionResult UploadPhoto(IFormFile photo)
        {

            var stream = photo.OpenReadStream();
            var result = CdnService.UploadPhoto(photo.FileName, stream);


            return new OkObjectResult(new
            {
                Url = result.Url
            });
        }
    }
}
