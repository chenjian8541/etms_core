using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryUserGetH5Request : RequestBase
    {
        public int? Year { get; set; }

        public int? Month { get; set; }

        public override string Validate()
        {
            return string.Empty;
        }
    }
}
