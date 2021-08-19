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

        public List<TeacherSalaryPayrollUserPerformanceView> PerformanceViews { get; set; }
    }

    public class TeacherSalaryPayrollUserPerformanceView
    {

        public EtTeacherSalaryPayrollUserPerformance TeacherSalaryPayrollUserPerformance { get; set; }

        public List<EtTeacherSalaryPayrollUserPerformanceDetail> PerformanceDetails { get; set; }
    }
}
