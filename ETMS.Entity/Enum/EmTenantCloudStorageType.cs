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

        static EmTenantCloudStorageType()
        {
            TenantCloudStorageTypeTags = new List<TenantCloudStorageTypeTag>();
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("sysConfig", 1));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("studentAvatar", 2));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("userAvatar", 3));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("studentTrack", 4));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("evaluateStudent", 5));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("gift", 6));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("studentLeave", 7));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("studentNotice", 8));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("studentHomework", 9));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("studentGrowthRecord", 10));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("studentWxMessage", 11));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("studentcheckOnLog", 12));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("micwebColumn", 13));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("micwebArticle", 14));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("mallGoods", 15));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("shareTemplate", 16));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("album", 17));
            TenantCloudStorageTypeTags.Add(new TenantCloudStorageTypeTag("agtPay", 18));
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
