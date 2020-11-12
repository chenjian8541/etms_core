using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class TeacherClassRecordEvaluateGetDetailOutput
    {
        public long ClassRecordId { get; set; }

        public long ClassId { get; set; }

        public string ClassName { get; set; }

        public string ClassOtDesc { get; set; }

        public string ClassTimeDesc { get; set; }

        public string WeekDesc { get; set; }

        public string CourseListDesc { get; set; }

        public string TeachersDesc { get; set; }

        public int TotalNeedEvaluateCount { get; set; }

        public int TotalEvaluateCount { get; set; }

        public DateTime CheckOt { get; set; }

        public string ClassRoomIdsDesc { get; set; }
    }
}
