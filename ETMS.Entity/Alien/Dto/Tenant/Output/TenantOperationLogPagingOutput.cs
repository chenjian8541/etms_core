using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.Tenant.Output
{
    public class TenantOperationLogPagingOutput
    {
        public string TenantName { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmPeopleType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 操作类型  <see cref="ETMS.Entity.Enum.EmUserOperationType"/>
        /// </summary>
        public string TypeDesc { get; set; }

        public string IpAddress { get; set; }

        public int ClientType { get; set; }

        public string OpContent { get; set; }

        public DateTime Ot { get; set; }

        public string ClientTypeDesc { get; set; }
    }
}
