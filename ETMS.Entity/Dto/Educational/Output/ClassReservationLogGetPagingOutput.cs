using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassReservationLogGetPagingOutput
    {
        public long Id { get; set; }

        public long ClassId { get; set; }

        public string ClassName { get; set; }

        public long CourseId { get; set; }

        public string CourseName { get; set; }

        public long ClassTimesId { get; set; }

        public long RuleId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

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
