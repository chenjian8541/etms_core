using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Request
{
    public class WxMiniEditUserInfoRequest: OpenParent2RequestBase
    {
        public string NickName { get; set; }

        public string AvatarUrl { get; set; }
    }
}
