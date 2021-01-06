using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common.Output;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.WebApi.Controllers.Common
{
    public class UploadFileBase642Action
    {
        public ResponseBase ProcessAction(UploadFileBase642Request request)
        {
            var fileExtension = string.Empty;
            var folderTag = string.Empty;
            switch (request.FileType)
            {
                case UploadFileType.Video:
                    fileExtension = ".mp4";
                    folderTag = AliyunOssFileTypeEnum.Video;
                    break;
                case UploadFileType.Audio:
                    fileExtension = ".mp3";
                    folderTag = AliyunOssFileTypeEnum.Audio;
                    break;
                default:
                    fileExtension = ".png";
                    folderTag = AliyunOssFileTypeEnum.Image;
                    break;
            }
            var strBase64 = request.FileData.Substring(request.FileData.IndexOf(",") + 1);
            var imgByte = Convert.FromBase64String(strBase64);
            var baseKey = $"{DateTime.Now.ToString("yyyyMMdd")}/{AliyunOssUtil.GetOneNewFileName()}{fileExtension}";
            var ossKey = AliyunOssUtil.PutObject(request.LoginTenantId, baseKey, folderTag, imgByte, imgByte.Length);
            return ResponseBase.Success(new UploadFileOutput()
            {
                Key = ossKey,
                Url = AliyunOssUtil.GetAccessUrlHttps(ossKey)
            });
        }
    }
}
