using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentRefreshTokenRequest : IValidate
    {
        /// <summary>
        /// 登录信息
        /// </summary>
        public string StrLoginInfo { get; set; }

        /// <summary>
        /// 前面信息
        /// </summary>
        public string StrSignature { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(StrLoginInfo) || string.IsNullOrEmpty(StrSignature))
            {
                return "凭证不能为空";
            }
            return string.Empty;
        }
    }
}
