using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryPayrollGoSettlementOutput
    {
        public long CId { get; set; }

        public int FinishCount { get; set; }

        public int FailCount { get; set; }
    }
}
