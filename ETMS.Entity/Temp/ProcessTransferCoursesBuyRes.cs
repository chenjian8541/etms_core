using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class ProcessTransferCoursesBuyRes
    {
        public StringBuilder BuyCourse { get; set; }

        public List<EtOrderDetail> OrderDetails { get; set; }

        public List<EtStudentCourseDetail> StudentCourseDetails { get; set; }

        public List<OneToOneClass> OneToOneClassLst { get; set; }

        public List<long> ChangeCourseIds { get; set; }
    }
}
