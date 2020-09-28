using ETMS.Entity.Dto.Common.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassBascGetOutput
    {
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        public string ClassName { get; set; }

        public int DefaultClassTimes { get; set; }

        public List<SelectItem> ClassRooms { get; set; }

        public List<SelectItem> ClassCourses { get; set; }

        public List<SelectItem> ClassTeachers { get; set; }
    }
}
