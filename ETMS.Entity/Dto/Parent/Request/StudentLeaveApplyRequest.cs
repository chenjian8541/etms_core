using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class StudentLeaveApplyRequest : ParentRequestBase
    {
        public string LeaveContent { get; set; }

        public List<string> LeaveMediasKeys { get; set; }

        public long StudentId { get; set; }
    }
}
