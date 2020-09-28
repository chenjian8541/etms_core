using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class ClassTimesPreInfo
    {
        public bool CourseListIsAlone { get; set; }

        public string CourseList { get; set; }

        public bool ClassRoomIdsIsAlone { get; set; }

        public string ClassRoomIds { get; set; }

        public bool TeachersIsAlone { get; set; }

        public string TeacherIds { get; set; }

        public int TeacherCount { get; set; }

        public string StudentIdsClass { get; set; }
    }
}
