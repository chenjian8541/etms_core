using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryFundsItemsGetOutput
    {
        public bool IsOpenContractPerformance { get; set; }

        public List<TeacherSalaryFundsItemOutput> DefaultItems { get; set; }

        public List<TeacherSalaryFundsItemOutput> CustomItems { get; set; }
    }
}
