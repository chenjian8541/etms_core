using ETMS.Entity.Config;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public class EmTenantCloudStorageType
    {
        public static List<TenantCloudStorageTypeTag> TenantCloudStorageTypeTags
        {
            get;
            private set;
        }

        public const string sysConfig = "sysConfig";
        public const string studentAvatar = "studentAvatar";
        public const string userAvatar = "userAvatar";
        public const string studentTrack = "studentTrack";
        public const string evaluateStudent = "evaluateStudent";
        public const string gift = "gift";
        public const string studentLeave = "studentLeave";
        public const string studentNotice = "studentNotice";
        public const string studentHomework = "studentHomework";
        public const string studentGrowthRecord = "studentGrowthRecord";
        public const string studentWxMessage = "studentWxMessage";
        public const string studentcheckOnLog = "studentcheckOnLog";
        public const string micwebColumn = "micwebColumn";
        public const string micwebArticle = "micwebArticle";
        public const string mallGoods = "mallGoods";
        public const string shareTemplate = "shareTemplate";
        public const string album = "album";
        public const string agtPay = "agtPay";
        public const string studentFaceKey = "studentFaceKey";
        public const string studentHomeworkAnswer = "studentHomeworkAnswer";
        public const string mallGoodsContent = "mallGoodsContent";
        public const string micwebArticleContent = "micwebArticleContent";
        public const string albumImg = "albumImg";
        public const string albumAudio = "albumAudio";
        public const string appConfig = "appConfig";
        public const string microWebConfig = "microWebConfig";

        public static string GetOssKeyPrefix(string fileTag, string rootFolder, int tenantId, DateTime now)
        {
            return $"{SystemConfig.ComConfig.OSSRootNewFolder}/{fileTag}/{rootFolder}/{tenantId}/{AliyunOssFileTypeEnum.STS}/{now.EtmsToDateString3()}/";
        }

        static EmTenantCloudStorageType()
        {
            TenantCloudStorageTypeTags = new List<TenantCloudStorageTypeTag>();
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(sysConfig, 1));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentAvatar, 2));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(userAvatar, 3));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentTrack, 4));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(evaluateStudent, 5));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(gift, 6));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentLeave, 7));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentNotice, 8));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentHomework, 9));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentGrowthRecord, 10));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentWxMessage, 11));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentcheckOnLog, 12));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(micwebColumn, 13));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(micwebArticle, 14));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(mallGoods, 15));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(shareTemplate, 16));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(album, 17));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(agtPay, 18));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentFaceKey, 19));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(studentHomeworkAnswer, 20));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(mallGoodsContent, 21));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(micwebArticleContent, 22));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(albumImg, 23));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(albumAudio, 24));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(appConfig, 25));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag(microWebConfig, 26));
        }
    }

    public class TenantCloudStorageTypeTag
    {
        public TenantCloudStorageTypeTag(string s, int t)
        {
            this.Tag = s;
            this.Type = t;
        }

        public string Tag { get; set; }

        public int Type { get; set; }
    }
}
