using ETMS.Entity.Enum;
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
            var imgByte = Convert.FromBase64String(strBase64);
            var now = DateTime.Now;
            var baseKeyPrefix = EmTenantCloudStorageType.GetOssKeyPrefix(EmTenantCloudStorageType.studentFaceKey, AliyunOssUtil.RootFolder, tenantId, now);
            var fullKey = $"{baseKeyPrefix}{AliyunOssUtil.GetOneNewFileName()}{ImageFaceFileExtension}";
            AliyunOssUtil.PutObject2(fullKey, imgByte, imgByte.Length);
            return fullKey;
        }

        public static string SaveStudentSearchFace(int tenantId, string strBase64, string fileType)
        {
            var imgByte = Convert.FromBase64String(strBase64);
            var baseKey = $"{DateTime.Now.ToString("yyyyMMdd")}/{AliyunOssUtil.GetOneNewFileName()}{ImageFaceFileExtension}";
            var ossKey = AliyunOssUtil.PutObjectTemp(tenantId, baseKey, fileType, imgByte, imgByte.Length);
            return ossKey;
        }
    }
}
