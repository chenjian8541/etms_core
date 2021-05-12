using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysSmsLog")]
    public class SysSmsLog: EManageEntity<long>
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public string Phone { get; set; }

        public int Type { get; set; }

        /// <summary> 
        /// 接收者类型   <see cref="ETMS.Entity.Enum.EtmsManage.EmPeopleType"/>
        /// </summary>
        public byte RetType { get; set; }

        public int DeCount { get; set; }

        public string SmsContent { get; set; }

        public DateTime Ot { get; set; }

        public byte Status { get; set; }

    }
}
