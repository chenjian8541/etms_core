using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp
{
    public class ProcessTransferCoursesOutRes
    {
        public List<EtOrderDetail> NewOrderDetailList { get; set; }

        public string OutCourseDesc { get; set; }

        public List<long> SourceOrderIds { get; set; }
    }
}
