using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class ParentBannerSaveRequest : RequestBase
    {
        public List<ParentBannerSet> ParentBannerSets { get; set; }

        public override string Validate()
        {
            if (ParentBannerSets != null && ParentBannerSets.Count > 6)
            {
                return "最多设置6张banner图片";
            }
            return base.Validate();
        }
    }

    public class ParentBannerSet
    {

        public string ImgKey { get; set; }

        public string UrlKey { get; set; }
    }
}
