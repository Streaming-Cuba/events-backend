using CloudinaryDotNet.Actions;

using Events.API.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Services.CDN
{
    public static class UploadResultExtensions
    {

        public static CdnUploadResult ToAbstractResult(this ImageUploadResult result)
        {
            return new CdnUploadResult()
            {
                Url = result.SecureUrl.ToString()
            };
        }
    }
}
