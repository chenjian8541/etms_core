using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentLoginRequest : IValidate
    {
        public string AgentCode { get; set; }
        public string Phone { get; set; }

        public string Pwd { get; set; }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public virtual string Validate()
        {
            AgentCode = AgentCode.Trim();
            Phone = Phone.Trim();
            Pwd = Pwd.Trim();
            if (string.IsNullOrEmpty(AgentCode))
            {
                return "代理商编码不能为空";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "手机号码不能为空";
            }
            if (string.IsNullOrEmpty(Pwd))
            {
                return "密码不能为空";
            }
            return string.Empty;
        }
    }
}
