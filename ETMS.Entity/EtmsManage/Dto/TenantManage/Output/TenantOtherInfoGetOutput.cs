using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Output
{
    public class TenantOtherInfoGetOutput
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// 机构代码
        /// </summary>
        public string TenantCode { get; set; }

        /// <summary>
        /// 机构电话
        /// </summary>
        public string TenantPhone { get; set; }

        public string HomeLogo1 { get; set; }

        public string HomeLogo2 { get; set; }

        public string LoginLogo1 { get; set; }

        public string LoginBg { get; set; }

        public string HomeLogo1Url { get; set; }

        public string HomeLogo2Url { get; set; }

        public string LoginLogo1Url { get; set; }

        public string LoginBgUrl { get; set; }

        public string TenantMyLink { get; set; }
    }
}
