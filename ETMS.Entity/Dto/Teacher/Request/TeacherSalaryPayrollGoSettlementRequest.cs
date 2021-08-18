using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryPayrollGoSettlementRequest : RequestBase
    {
        public string Name { get; set; }

        public List<long> UserIds { get; set; }

        public DateTime? PayDate { get; set; }

        /// <summary>
        /// 查询时间
        /// </summary>
        public List<string> Ot { get; set; }

        private DateTime? _startOt;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartOt
        {
            get
            {
                if (_startOt != null)
                {
                    return _startOt;
                }
                if (Ot == null || Ot.Count == 0)
                {
                    return null;
                }
                _startOt = Convert.ToDateTime(Ot[0]);
                return _startOt;
            }
        }

        private DateTime? _endOt;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndOt
        {
            get
            {
                if (_endOt != null)
                {
                    return _endOt;
                }
                if (Ot == null || Ot.Count < 2)
                {
                    return null;
                }
                _endOt = Convert.ToDateTime(Ot[1]).AddDays(1); ;
                return _endOt;
            }
        }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入结算名称";
            }
            if (StartOt == null || EndOt == null)
            {
                return "请选择结算区间";
            }
            if (StartOt > EndOt)
            {
                return "开始时间不能大于结束时间";
            }
            if ((EndOt.Value - StartOt.Value).TotalDays > 400)
            {
                return "结算区间不能大于一年";
            }
            if (UserIds == null || UserIds.Count == 0)
            {
                return "请选择结算对象";
            }
            if (UserIds.Count > 20)
            {
                return "一次性最多结算20名员工";
            }
            if (PayDate != null && !PayDate.Value.IsEffectiveDate())
            {
                return "请选择发薪日期";
            }
            return base.Validate();
        }
    }
}
