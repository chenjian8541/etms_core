using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantStatistics2")]
    public class SysTenantStatistics2 : EManageEntity<long>
    {
        public int TenantId { get; set; }

        public int StudentReadCount { get; set; }

        public int StudentPotentialCount { get; set; }

        public int StudentHistoryCount { get; set; }

        public int TeacherCount { get; set; }
    }
}
