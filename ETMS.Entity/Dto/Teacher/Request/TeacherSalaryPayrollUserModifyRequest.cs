using ETMS.Entity.Common;
using ETMS.Entity.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryPayrollUserModifyRequest : RequestBase
    {
        public long TeacherSalaryPayrollIdId { get; set; }

        public long PayrollUserId { get; set; }

        public List<TeacherSalaryUpdatePayValue> FixedSalaryList { get; set; }

        public List<TeacherSalaryUpdatePayValue> PerformanceSalaryList { get; set; }

        public override string Validate()
        {
            if (TeacherSalaryPayrollIdId <= 0 || PayrollUserId <= 0)
            {
                return "请求数据格式错误";
            }
            if (FixedSalaryList == null || FixedSalaryList.Count == 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }

}