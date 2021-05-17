using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Output
{
    public class SysTenantOperationLogPagingOutput
    {
        //public int AgentId { get; set; }

        //public string AgentName { get; set; }

        //public string AgentPhone { get; set; }

        //public long TenantId { get; set; }

        //public string TenantName { get; set; }

        //public string TenantPhone { get; set; }

        public long UserId { get; set; }

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
