using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassRecordGetPagingSimpleOutput
    {
        public long CId { get; set; }

        public long ClassId { get; set; }

        public string ClassName { get; set; }

        public DateTime CheckOt { get; set; }

        public string ClassOtDesc { get; set; }

        public string ClassTimeDesc { get; set; }

        public string CourseListDesc { get; set; }

        public string ClassRoomIdsDesc { get; set; }

        public string TeachersDesc { get; set; }

        public long CheckUserId { get; set; }

        public string CheckUserDesc { get; set; }

        public int AttendNumber { get; set; }

        public int NeedAttendNumber { get; set; }

        public string ClassTimes { get; set; }

        public string DeSum { get; set; }

        public string WeekDesc { get; set; }

        public string Label { get; set; }

        public long Value { get; set; }
    }
}
