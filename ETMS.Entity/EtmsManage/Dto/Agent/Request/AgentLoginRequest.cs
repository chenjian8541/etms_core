using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentLoginRequest : IValidate
    {
        public string Phone { get; set; }

        public string Pwd { get; set; }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public virtual string Validate()
        {
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
