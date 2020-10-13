using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class UserLoginBySmsH5Output
    {
        /// <summary>
        /// 授权Token
        /// </summary>
        public string Token { get; set; }

        public DateTime ExpiresTime { get; set; }
    }
}
