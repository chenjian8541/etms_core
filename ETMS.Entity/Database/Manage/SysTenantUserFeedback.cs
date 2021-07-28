using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysTenantUserFeedback")]
    public class SysTenantUserFeedback : EManageEntity<long>
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public DateTime Ot { get; set; }

        public string LinkPhone { get; set; }

        public string ProblemType { get; set; }

        public string ProblemLevel { get; set; }

        public string ProblemTheme { get; set; }

        public string ProblemContent { get; set; }
    }
}
