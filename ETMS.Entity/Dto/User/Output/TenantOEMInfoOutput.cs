using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class TenantOEMInfoOutput
    {
        public string HomeLogo1Url { get; set; }

        public string HomeLogo2Url { get; set; }

        public string LoginLogo1Url { get; set; }

        public string LoginBgUrl { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public bool IsHideKeFu { get; set; }

        public string WebSiteTitle { get; set; }

        public string KefuMobile { get; set; }
    }
}
