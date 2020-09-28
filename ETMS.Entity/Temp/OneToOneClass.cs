using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class OneToOneClass
    {
        public string Name { get; set; }

        public long CourseId { get; set; }

        public int StudentNums { get; set; }

        public byte Type { get; set; }

        public List<OneToOneClassStudent> Students { get; set; }
    }

    public class OneToOneClassStudent
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }
    }
}
