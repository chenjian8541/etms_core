using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Output
{
    public class UserFeedbackPagingOutput
    {
        public int AgentId { get; set; }

        public int TenantId { get; set; }

        public string TenantName { get; set; }

        public string TenantPhone { get; set; }

        public long UserId { get; set; }

        public DateTime Ot { get; set; }

        public string LinkPhone { get; set; }

        public string ProblemType { get; set; }

        public string ProblemLevel { get; set; }

        public string ProblemTheme { get; set; }

        public string ProblemContent { get; set; }
    }
}
