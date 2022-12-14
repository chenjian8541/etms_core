using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryPayrollUserGetRequest : RequestBase
    {
        public long TeacherSalaryPayrollIdId { get; set; }

        public long PayrollUserId { get; set; }

        public override string Validate()
        {
            if (TeacherSalaryPayrollIdId <= 0 || PayrollUserId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
