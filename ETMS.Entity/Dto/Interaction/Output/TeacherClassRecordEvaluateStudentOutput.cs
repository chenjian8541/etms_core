using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class TeacherClassRecordEvaluateStudentOutput
    {
        public long ClassRecordId { get; set; }

        public long ClassRecordStudentId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 到课状态  <see cref="ETMS.Entity.Enum.EmClassStudentCheckStatus"/>
        /// </summary>
        public byte StudentCheckStatus { get; set; }

        public string StudentCheckStatusDesc { get; set; }

        public int EvaluateCount { get; set; }

        public int EvaluateReadCount { get; set; }

        public bool IsCanEvaluate { get; set; }
    }
}
