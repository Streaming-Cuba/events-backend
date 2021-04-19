using Events.API.Models;
using System.IO;

namespace Events.API.Services.CDN
{
    public interface ICdnService
    {
        CdnUploadResult UploadPhoto(string name, Stream stream, string tags = "", string folder = "");
    }
}
