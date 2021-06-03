using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class MicroWebBannerGetOutput
    {
        public bool IsShowInHome { get; set; }

        public List<MicroWebBannerGetImage> Images { get; set; }
    }

    public class MicroWebBannerGetImage
    {
        public string ImgUrl { get; set; }
        public string ImgKey { get; set; }
    }
}
