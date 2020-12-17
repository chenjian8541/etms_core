using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantTxCloudUCount")]
    public class SysTenantTxCloudUCount : EManageEntity<long>
    {
        public int TenantId { get; set; }

        public DateTime Ot { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantTxCloudUCountType"/>
        /// </summary>
        public byte Type { get; set; }

        public int UseCount { get; set; }
    }
}
