using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryUserDetailGetH5Output
    {
        public string Name { get; set; }

        public string OtDesc { get; set; }

        public string PayDateDesc { get; set; }

        public List<TeacherSalaryUserDetailGetH5Item> Items { get; set; }
    }

    public class TeacherSalaryUserDetailGetH5Item
    {
        public byte Type { get; set; }

        public string Name { get; set; }

        public string AmountSum { get; set; }
    }
}
