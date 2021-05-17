using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantStatistics")]
    public class SysTenantStatistics : EManageEntity<int>
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public int StudentCount1 { get; set; }

        public int StudentCount2 { get; set; }

        public int StudentCount3 { get; set; }

        public int UserCount { get; set; }

        public int OrderCount { get; set; }

        public int ClassRecordCount { get; set; }

        public int ClassCount1 { get; set; }

        public int ClassCount2 { get; set; }
    }
}
