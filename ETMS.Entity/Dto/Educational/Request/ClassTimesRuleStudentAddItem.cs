using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesRuleStudentAddItem
    {
        public long StudentId { get; set; }

        public long CourseId { get; set; }

        public string StudentName { get; set; }
    }
}
