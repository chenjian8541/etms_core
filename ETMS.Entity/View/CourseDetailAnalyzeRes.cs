using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class CourseDetailAnalyzeRes
    {
        public List<EtStudentCourse> NewCourse { get; set; }

        public bool IsCourseNotEnough { get; set; }
    }
}
