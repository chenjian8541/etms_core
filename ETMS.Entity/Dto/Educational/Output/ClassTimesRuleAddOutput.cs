using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassTimesRuleAddOutput
    {
        public bool IsLimit { get; set; }

        public bool IsLimitStudent { get; set; }

        public string LimitTeacherName { get; set; }

        public string LimitStudentName { get; set; }
    }
}
