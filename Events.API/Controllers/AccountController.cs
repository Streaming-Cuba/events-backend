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
    [Route("/api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController() { }

        [HttpPost]
        public IActionResult CreateAccount(IFormFile photo)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("/role")]
        public IActionResult CreateRole()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("/permission")]
        public IActionResult CreatePermission()
        {
            throw new NotImplementedException();
        }
    }
}
