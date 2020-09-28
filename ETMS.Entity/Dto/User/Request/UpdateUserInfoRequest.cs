using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class UpdateUserInfoRequest : RequestBase
    {
        /// <summary>
        /// 头像Key
        /// </summary>
        public string AvatarKey { get; set; }

        public string NickName { get; set; }
    }
}
