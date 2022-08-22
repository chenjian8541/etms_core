using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational.Output
{
    public class ClassTimesRuleEditOutput
    {
        public bool IsLimit { get; set; }

        public bool IsLimitStudent { get; set; }

        public bool IsLimitClassRoom { get; set; }

        public string LimitTeacherName { get; set; }

        public string LimitStudentName { get; set; }

        public string LimitClassRoomName { get; set; }
    }
}
