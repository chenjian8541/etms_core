using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Database.Source
{
    public class BaseTeacherSalaryClassTimes : Entity<long>
    {
        public long TeacherId { get; set; }

        public long ClassId { get; set; }

        public long ClassRecordId { get; set; }

        public DateTime Ot { get; set; }

        public byte Week { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

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

        public int TryCalssEffectiveCount { get; set; }

        public int MakeUpEffectiveCount { get; set; }
    }
}
