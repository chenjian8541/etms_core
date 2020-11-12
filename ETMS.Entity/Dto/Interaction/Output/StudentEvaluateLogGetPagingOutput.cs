using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class StudentEvaluateLogGetPagingOutput
    {
        public long EvaluateTeacherRecordId { get; set; }

        public DateTime Ot { get; set; }

        public string TeacherName { get; set; }

        public string ClassName { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string ClassOtDesc { get; set; }

        public string ClassTimeDesc { get; set; }

        public string WeekDesc { get; set; }


        public int StarValue { get; set; }
    }
}
