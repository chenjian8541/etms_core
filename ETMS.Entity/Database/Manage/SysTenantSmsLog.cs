using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantSmsLog")]
    public class SysTenantSmsLog: EManageEntity<int>
    {
        public int TenantId { get; set; }

        public int AgentId { get; set; }

        /// <summary>
        ///类型 <see cref="Enum.EtmsManage.EmSysTenantSmsLogChangeType"/>
        /// </summary>
        public int ChangeType { get; set; }

        public int ChangeCount { get; set; }

        public decimal Sum { get; set; }

        public DateTime Ot { get; set; }

    }
}
