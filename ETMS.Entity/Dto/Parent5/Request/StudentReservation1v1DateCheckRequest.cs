using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ETMS.Entity.Dto.Parent5.Request
{
    public class StudentReservation1v1DateCheckRequest : ParentRequestBase
    {
        public long TeacherId { get; set; }

        public long CourseId { get; set; }

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
            if (CourseId <= 0)
            {
                return "请选择课程";
            }
            if (TeacherId <= 0)
            {
                return "请选择老师";
            }
            if (Ot == null || Ot.Count != 2)
            {
                return "时间格式不正确";
            }
            return base.Validate();
        }
    }
}
