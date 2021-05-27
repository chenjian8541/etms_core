using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;


namespace ETMS.Entity.Database.Manage
{
    [Table("SysSmsTemplate")]
    public class SysSmsTemplate : EManageEntity<int>
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysSmsTemplateType"/>
        /// </summary>
        public int Type { get; set; }

        public string SmsContent { get; set; }

        public DateTime? HandleOt { get; set; }

        public long? HandleUser { get; set; }

        public string HandleContent { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EtmsManage.EmSysSmsTemplateHandleStatus"/>
        /// </summary>
        public byte HandleStatus { get; set; }

        public DateTime CreateOt { get; set; }

        public DateTime UpdateOt { get; set; }
    }
}
