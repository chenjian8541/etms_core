using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Educational3.Request
{
    public class AchievementStudentIncreaseGetRequest : RequestBase
    {
        public long StudentId { get; set; }

        public long SubjectId { get; set; }

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
                _endOt = Convert.ToDateTime(Ot[1]); ;
                return _endOt;
            }
        }

        public override string Validate()
        {
            if (this.StudentId <= 0)
            {
                return "请选择学员";
            }
            if (this.SubjectId <= 0)
            {
                return "请选择科目";
            }
            if (Ot == null || !Ot.Any() || Ot.Count != 2)
            {
                return "请选择时间";
            }
            var diffDay = this.EndOt.Value - this.StartOt.Value;
            if (diffDay.TotalDays > 60)
            {
                return "时间间隔最多60天";
            }
            return string.Empty;
        }
    }
}
