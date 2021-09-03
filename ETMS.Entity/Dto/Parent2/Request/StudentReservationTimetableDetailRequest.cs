using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent2.Request
{
    public class StudentReservationTimetableDetailRequest : ParentRequestBase
    {
        public long StudentId { get; set; }

        public List<long> StudentCourseIds { get; set; }

        public long? TeacherId { get; set; }

        public DateTime SldTime { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere2());
            condition.Append($" AND ClassType = {EmClassType.OneToMany} AND ReservationType = {EmBool.True} AND ClassOt = '{SldTime.EtmsToDateString()}'");
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
            if (!SldTime.IsEffectiveDate())
            {
                return "时间格式不正确";
            }
            return string.Empty;
        }
    }
}
