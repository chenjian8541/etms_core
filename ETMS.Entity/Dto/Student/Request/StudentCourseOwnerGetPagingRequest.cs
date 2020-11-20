using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseOwnerGetPagingRequest : RequestPagingBase
    {
        /// <summary>
        /// 选择的课程
        /// </summary>
        public List<long> MyCourseIds { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (MyCourseIds != null && MyCourseIds.Count > 0)
            {
                if (MyCourseIds.Count == 1)
                {
                    condition.Append($" AND CourseId = {MyCourseIds[0]}");
                }
                else
                {
                    condition.Append($" AND CourseId IN ({string.Join(',', MyCourseIds)})");
                }
            }
            condition.Append($" AND StudentType = {EmStudentType.ReadingStudent} AND [Status] = {EmStudentCourseStatus.Normal} ");
            return condition.ToString();
        }
    }
}