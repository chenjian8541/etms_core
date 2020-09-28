using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Common;
using ETMS.Entity.Dto.Common.Output;
using ETMS.Utility;
using ETMS.WebApi.Lib;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers.Common
{
    public class UploadImgAction
    {
        public async Task<ResponseBase> ProcessAction(IFormCollection collection, IHttpContextAccessor _httpContextAccessor, AppSettings appSettings)
        {
            var response = ResponseBase.UnKnownError();
            if (collection.Files.Count == 0)
            {
                return response.GetResponseError("请选择文件");
            }
            var file = collection.Files[0];
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (string.IsNullOrEmpty(fileExtension) || Array.IndexOf(appSettings.StaticFilesConfig.UploadImageFileTypeLimit.Split('|'), fileExtension) == -1)
            {
                return response.GetResponseError("请上传图片文件");
            }
            if (file.Length > appSettings.StaticFilesConfig.UploadFileSizeLimit)
            {
                return response.GetResponseError("文件大小被限制");
            }
            var newFileName = $"{System.Guid.NewGuid().ToString().Replace("-", "")}{fileExtension}";
            var imgFolderPreInfo = FileHelper.PreProcessImgFolder(appSettings.StaticFilesConfig.ServerPath);
            var filePath = Path.Combine(imgFolderPreInfo.Item1, newFileName);
            var urlKey = $"{imgFolderPreInfo.Item2}{newFileName}";
            using (var content = file.OpenReadStream())
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await content.CopyToAsync(stream);
                    return ResponseBase.Success(new UploadImgOutput()
                    {
                        Key = urlKey,
                        Url = UrlHelper.GetUrl(_httpContextAccessor, appSettings.StaticFilesConfig.VirtualPath, urlKey)
                    });
                }
            }
        }
    }
}
