using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Business.Common
{
    public class ImageLib
    {
        private const string ImageFaceFileExtension = ".png";

        public static string SaveStudentFace(int tenantId, string strBase64)
        {
            strBase64 = strBase64.Substring(strBase64.IndexOf(",") + 1);
            var imgByte = Convert.FromBase64String(strBase64);
            var baseKey = $"{DateTime.Now.ToString("yyyyMMdd")}/{AliyunOssUtil.GetOneNewFileName()}{ImageFaceFileExtension}";
            var ossKey = AliyunOssUtil.PutObject(tenantId, baseKey, AliyunOssFileTypeEnum.ImageStudentFace, imgByte, imgByte.Length);
            return ossKey;
        }

        public static string SaveStudentSearchFace(int tenantId, string strBase64, string fileType)
        {
            strBase64 = strBase64.Substring(strBase64.IndexOf(",") + 1);
            var imgByte = Convert.FromBase64String(strBase64);
            var baseKey = $"{DateTime.Now.ToString("yyyyMMdd")}/{AliyunOssUtil.GetOneNewFileName()}{ImageFaceFileExtension}";
            var ossKey = AliyunOssUtil.PutObjectTemp(tenantId, baseKey, fileType, imgByte, imgByte.Length);
            return ossKey;
        }
    }
}
