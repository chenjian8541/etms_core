using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Output
{
    public class SysSmsTemplateGetPagingOutput
    {
        public int Id { get; set; }
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public string TenantName { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysSmsTemplateType"/>
        /// </summary>
        public int Type { get; set; }

        public string TypeDesc { get; set; }

        public string SmsContent { get; set; }

        public DateTime? HandleOt { get; set; }

        public string HandleContent { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EtmsManage.EmSysSmsTemplateHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        public string HandleStatusDesc { get; set; }

        public DateTime CreateOt { get; set; }

        public DateTime UpdateOt { get; set; }
    }
}
