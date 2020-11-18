using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class EvaluateTeacherGetOutput
    {
        public long Id { get; set; }

        public string ClassName { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string TeachersDesc { get; set; }

        /// <summary>
        /// 到课状态  <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        public string StudentCheckStatusDesc { get; set; }

        public string ClassOtDesc { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public byte Week { get; set; }

        /// <summary>
        /// 周
        /// </summary>
        public string WeekDesc { get; set; }

        /// <summary>
        /// 是否点评(老师点评)
        /// </summary>
        public bool IsBeEvaluate { get; set; }

        /// <summary>
        /// 是否允许点评
        /// </summary>
        public bool IsCanEvaluate { get; set; }
    }
}
