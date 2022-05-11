using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent2.Output
{
    public class StudentReservationLogGetPaging2Output
    {
        public long Id { get; set; }

        public long ClassId { get; set; }

        /// <summary>
        /// 班级类型  <see cref="ETMS.Entity.Enum.EmClassType"/>
        /// </summary>
        public byte ClassType { get; set; }

        public string ClassName { get; set; }

        public long CourseId { get; set; }

        public string CourseName { get; set; }

        public long ClassTimesId { get; set; }

        public long RuleId { get; set; }

        public byte Week { get; set; }

        public string WeekDesc { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 结束
        /// </summary>
        public int EndTime { get; set; }

        public string TimeDesc { get; set; }

        public string ClassOt { get; set; }

        public DateTime CreateOt { get; set; }

        /// <summary>
        ///  状态  <see cref=" ETMS.Entity.Enum.EmClassTimesReservationLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }
    }
}
