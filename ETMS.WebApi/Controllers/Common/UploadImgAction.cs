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
        public async Task<ResponseBase> ProcessAction(IFormCollection collection, AppSettings appSettings, int tenantId)
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
            var baseKey = $"{DateTime.Now.ToString("yyyyMMdd")}/{AliyunOssUtil.GetOneNewFileName()}{fileExtension}";
            using (var content = file.OpenReadStream())
            {
                var ossKey = AliyunOssUtil.PutObject(tenantId, baseKey, AliyunOssFileTypeEnum.Image, content);
                return ResponseBase.Success(new UploadFileOutput()
                {
                    Key = ossKey,
                    Url = AliyunOssUtil.GetAccessUrlHttps(ossKey)
                });
            }
        }
    }
}
