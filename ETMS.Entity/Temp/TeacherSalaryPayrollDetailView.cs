using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Temp
{
    public class TeacherSalaryPayrollDetailView
    {
        public EtTeacherSalaryPayrollUser TeacherSalaryPayrollUser { get; set; }

        public List<EtTeacherSalaryPayrollUserDetail> TeacherSalaryPayrollUserDetails { get; set; }

        public List<EtTeacherSalaryPayrollUserPerformance> TeacherSalaryPayrollUserPerformances { get; set; }
    }
}
