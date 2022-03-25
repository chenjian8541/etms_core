using ETMS.Entity.Dto.User.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.User.Output
{
    public class AlUserLoginOutput
    {
        /// <summary>
        /// 授权Token
        /// </summary>
        public string Token { get; set; }

        public DateTime ExpiresTime { get; set; }

        public PermissionOutput Permission { get; set; }

        public long UId { get; set; }

        public int HId { get; set; }
    }
}
