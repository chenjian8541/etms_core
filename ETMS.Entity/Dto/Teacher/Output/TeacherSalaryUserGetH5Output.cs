using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryUserGetH5Output
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public string TotalSalary { get; set; }

        public List<TeacherSalaryUserGetH5UserDetail> SalaryDetails { get; set; }
    }

    public class TeacherSalaryUserGetH5UserDetail
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string OtDesc { get; set; }

        public string PayDateDesc { get; set; }

        public string PayItemSum { get; set; }
    }
}
