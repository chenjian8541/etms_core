using ETMS.Entity.Config.Router;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Output
{
    public class UserLoginOutput
    {
        /// <summary>
        /// 授权Token
        /// </summary>
        public string Token { get; set; }

        public DateTime ExpiresTime { get; set; }

        public PermissionOutput Permission { get; set; }
    }
}
