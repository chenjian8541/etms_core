using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent.Output
{
    public class StudentPhoneCheckOutput
    {
        public bool IsHas { get; set; }

        public StudentPhoneInfo StudentInfo { get; set; }
    }

    public class StudentPhoneInfo {

        public string StudentName { get; set; }
    }
}
