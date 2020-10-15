using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentChangPwdRequest: AgentRequestBase
    {
        public string NewPwd { get; set; }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            //if (string.IsNullOrEmpty(SmsCode))
            //{
            //    return "请输入验证码";
            //}
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
