using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent2.Request
{
    public class StudentReservationTimetableRequest : ParentRequestBase
    {
        public long StudentId { get; set; }

        public long? TeacherId { get; set; }

        public List<long> StudentCourseIds { get; set; }

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

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere2());
            condition.Append($" AND ClassType = {EmClassType.OneToMany} AND ReservationType = {EmBool.True} AND ClassOt >= '{StartOt.Value.EtmsToDateString()}' AND ClassOt < '{EndOt.Value.EtmsToDateString()}'");
            condition.Append($" AND (StudentIdsClass NOT LIKE '%,{StudentId},%' OR StudentIdsClass IS NULL)");
            if (TeacherId != null)
            {
                condition.Append($" AND Teachers LIKE '%,{TeacherId.Value},%'");
            }
            if (StudentCourseIds.Count == 1)
            {
                condition.Append($" AND CourseList LIKE '%,{StudentCourseIds[0]},%'");
            }
            else
            {
                var tempSql = new StringBuilder();
                for (var i = 0; i < StudentCourseIds.Count; i++)
                {
                    if (i == StudentCourseIds.Count - 1)
                    {
                        tempSql.Append($" CourseList LIKE '%,{StudentCourseIds[i]},%'");
                    }
                    else
                    {
                        tempSql.Append($" CourseList LIKE '%,{StudentCourseIds[i]},%' OR");
                    }
                }
                condition.Append($" AND ({tempSql})");
            }
            return condition.ToString();
        }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            if (Ot == null || Ot.Count != 2)
            {
                return "时间格式不正确";
            }
            return string.Empty;
        }
    }
}
