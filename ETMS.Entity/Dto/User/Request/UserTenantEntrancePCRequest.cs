using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.User.Request
{
    public class UserTenantEntrancePCRequest
    {
        public int TenantId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string Phone { set; get; }

        /// <summary>
        /// 客户端类型  <see cref="EmUserOperationLogClientType"/>
        /// </summary>
        public int LoginClientType { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }
    }
}