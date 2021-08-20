using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryPayrollSetOKRequest : RequestBase
    {
        public long CId { get; set; }

        public DateTime? PayDate { get; set; }

        public byte PayType { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (PayDate == null)
            {
                return "请选择发薪日期";
            }
            if (!PayDate.Value.IsEffectiveDate())
            {
                return "请选择有效的发薪日期";
            }
            return string.Empty;
        }
    }
}
