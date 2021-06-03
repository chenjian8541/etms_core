using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebBannerSaveRequest : RequestBase
    {
        public bool IsShowInHome { get; set; }

        public List<MicroWebBannerSetInPut> BannerSets { get; set; }

        public override string Validate()
        {
            if (BannerSets != null && BannerSets.Count > 6)
            {
                return "最多设置6张banner图片";
            }
            return base.Validate();
        }
    }

    public class MicroWebBannerSetInPut
    {

        public string ImgKey { get; set; }

        public string UrlKey { get; set; }
    }
}
