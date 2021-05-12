using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Output
{
    public class SysSmsLogPagingOutput
    {
        public long Id { get; set; }

        public long AgentId { get; set; }

        public string AgentName { get; set; }

        public string AgentPhone { get; set; }

        public long TenantId { get; set; }

        public string TenantName { get; set; }

        public string TenantPhone { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// 接收者类型
        /// </summary>
        public byte RetType { get; set; }

        public int DeCount { get; set; }

        public string SmsContent { get; set; }

        public DateTime Ot { get; set; }

        public byte Status { get; set; }

        public string TypeDesc { get; set; }

    }
}
