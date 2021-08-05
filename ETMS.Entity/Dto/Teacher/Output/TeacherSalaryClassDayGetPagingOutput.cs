using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Output
{
    public class TeacherSalaryClassDayGetPagingOutput
    {
        public long Id { get; set; }

        public long TeacherId { get; set; }

        public string TeacherName { get; set; }

        public string TeacherPhone { get; set; }

        public long ClassId { get; set; }

        public string ClassName { get; set; }

        public DateTime Ot { get; set; }

        public decimal TeacherClassTimes { get; set; }

        public decimal StudentClassTimes { get; set; }

        public decimal DeSum { get; set; }

        public int ArrivedAndBeLateCount { get; set; }

        public int ArrivedCount { get; set; }

        public int BeLateCount { get; set; }

        public int LeaveCount { get; set; }

        public int NotArrivedCount { get; set; }

        public int TryCalssStudentCount { get; set; }

        public int MakeUpStudentCount { get; set; }
    }
}
