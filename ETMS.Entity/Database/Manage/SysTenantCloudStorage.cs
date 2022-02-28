using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantCloudStorage")]
    public class SysTenantCloudStorage : EManageEntity<int>
    {
        public int TenantId { get; set; }

        public int AgentId { get; set; }

        /// <summary>
        /// <see cref="EmTenantCloudStorageType"/>
        /// </summary>
        public int Type { get; set; }

        public decimal ValueMB { get; set; }

        public decimal ValueGB { get; set; }

        public DateTime LastModified { get; set; }
    }
}
