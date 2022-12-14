using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class GetTenantInfoOutput
    {
        /// <summary>
        /// 机构名称
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// 机构昵称
        /// </summary>
        public string TenantNickName { get; set; }

        /// <summary>
        /// 机构简介
        /// </summary>
        public string TenantDescribe { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string TenantLinkName { get; set; }

        /// <summary>
        /// 联系号码
        /// </summary>
        public string TenantLinkPhone { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string TenantAddress { get; set; }

        public string ParentHtmlTitle { get; set; }

        public string ParentLoginImage { get; set; }
    }
}
