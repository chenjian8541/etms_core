using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class ChangeTenantUserPwdRequest : IValidate
    {
        public string CheckNo { get; set; }

        public string TNo { get; set; }

        public string UNo { get; set; }

        public string NewPwd { get; set; }


        /// <summary>
        /// 客户端类型  <see cref="EmUserOperationLogClientType"/>
        /// </summary>
        public int LoginClientType { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(CheckNo))
            {
                return "数据校验不合法";
            }
            if (string.IsNullOrEmpty(TNo))
            {
                return "数据校验不合法";
            }
            if (string.IsNullOrEmpty(UNo))
            {
                return "数据校验不合法";
            }
            if (string.IsNullOrEmpty(NewPwd))
            {
                return "请输入新密码";
            }
            if (NewPwd.Length < 5 || NewPwd.Length > 20)
            {
                return "请输入5-20位的密码";
            }
            return string.Empty;
        }
    }
}