using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class TenantShowSaveRequest : RequestBase
    {
        public string SmsCode { get; set; }

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

        public string ParentTitle { get; set; }

        public string ParentLoginImage { get; set; }

        public string TeacherTitle { get; set; }

        public string TeacherLoginImage { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SmsCode))
            {
                return "请先验证管理员身份";
            }
            return base.Validate();
        }
    }
}
