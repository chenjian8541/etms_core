using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Dto.Common.Output;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers.Common
{
    public class UploadImg2Action
    {
        public ResponseBase ProcessAction(UploadImg2Request request)
        {
            var fileExtension = ".png";
            var strBase64 = request.ImgData.Substring(request.ImgData.IndexOf(",") + 1);
            var imgByte = Convert.FromBase64String(strBase64);
            var baseKey = $"{DateTime.Now.ToString("yyyyMMdd")}/{AliyunOssUtil.GetOneNewFileName()}{fileExtension}";
            var ossKey = AliyunOssUtil.PutObject(request.LoginTenantId, baseKey, AliyunOssFileTypeEnum.Image, imgByte, imgByte.Length);
            return ResponseBase.Success(new UploadFileOutput()
            {
                Key = ossKey,
                Url = AliyunOssUtil.GetAccessUrlHttps(ossKey)
            });
        }
    }
}
