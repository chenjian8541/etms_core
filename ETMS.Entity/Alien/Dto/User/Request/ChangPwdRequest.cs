using ETMS.Entity.Alien.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.User.Request
{
    public class ChangPwdRequest : AlienRequestBase
    {
        public string NewPwd { get; set; }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
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
