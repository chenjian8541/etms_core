using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Common.Output;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers.Common
{
    public class UploadFileBase64Action
    {
        public ResponseBase ProcessAction(IHttpContextAccessor _httpContextAccessor, AppSettings appSettings, UploadFileBase64Request request)
        {
            var fileExtension = string.Empty;
            var folderTag = string.Empty;
            switch (request.FileType)
            {
                case UploadFileType.Video:
                    fileExtension = ".mp4";
                    folderTag = "video";
                    break;
                case UploadFileType.Audio:
                    fileExtension = ".mp3";
                    folderTag = "audio";
                    break;
                default:
                    fileExtension = ".png";
                    folderTag = "img";
                    break;
            }
            var newFileName = $"{System.Guid.NewGuid().ToString().Replace("-", "")}{fileExtension}";
            var imgFolderPreInfo = FileHelper.PreProcessFolder(appSettings.StaticFilesConfig.ServerPath, folderTag);
            var filePath = Path.Combine(imgFolderPreInfo.Item1, newFileName);
            var urlKey = $"{imgFolderPreInfo.Item2}{newFileName}";
            var strBase64 = request.FileData.Substring(request.FileData.IndexOf(",") + 1);
            var imgByte = Convert.FromBase64String(strBase64);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                stream.Write(imgByte, 0, imgByte.Length);
                return ResponseBase.Success(new UploadFileOutput()
                {
                    Key = urlKey,
                    Url = UrlHelper.GetUrl(_httpContextAccessor, appSettings.StaticFilesConfig.VirtualPath, urlKey)
                });
            }
        }
    }
}
