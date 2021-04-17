using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;
using Events.API.Models;

namespace Events.API.Services.CDN
{
    public class CloudinaryService : ICdnService
    {
        const string CLOUDINARY_NAME_FIELD = "CLOUDINARY_NAME";
        const string CLOUDINARY_API_KEY_FIELD = "CLOUDINARY_API_KEY";
        const string CLOUDINARY_API_SECRET_FIELD = "CLOUDINARY_API_SECRET";
        public Cloudinary Cloudinary { get; }
        public Account Account { get;}

        public CloudinaryService(IConfiguration configuration)
        {
            var name = configuration[CLOUDINARY_NAME_FIELD];
            var apiKey = configuration[CLOUDINARY_API_KEY_FIELD];
            var apiSecret = configuration[CLOUDINARY_API_SECRET_FIELD];



            Account = new Account(name, apiKey, apiSecret);
            Cloudinary = new Cloudinary(Account);

           
        }
        public CdnUploadResult UploadPhoto(string name, Stream stream, string tags = "", string folder = "")
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new InvalidProgramException("You should provide a value for name");

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(webpFileName, stream),
                Tags = tags,
                Folder = folder,
            };
            var uploadResult = Cloudinary.Upload(uploadParams);

            return uploadResult.ToAbstractResult();
        }
    }
}
