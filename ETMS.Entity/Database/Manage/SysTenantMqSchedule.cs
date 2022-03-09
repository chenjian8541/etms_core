using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// mq 调度
    /// </summary>
    [Table("SysTenantMqSchedule")]
    public class SysTenantMqSchedule : EManageEntity<long>
    {
        public int TenantId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantMqScheduleType"/>
        /// </summary>
        public int Type { get; set; }

        public string SendContent { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime ExTime { get; set; }
    }
}
