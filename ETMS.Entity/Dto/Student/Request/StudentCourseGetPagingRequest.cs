using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseGetPagingRequest : RequestPagingBase
    {
        /// <summary>
        /// 学员信息
        /// </summary>
        public string StudentKey { get; set; }

        /// <summary>
        /// 消课方式  <see cref="<see cref="ETMS.Entity.Enum.EmDeClassTimesType"/>"/>
        /// </summary>
        public byte? DeType { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EmStudentCourseStatus"/>
        /// </summary>
        public byte? Status { get; set; }

        public long? CourseId { get; set; }

        public bool? IsQueryShort { get; set; }

        /// <summary>
        /// 剩余课时
        /// </summary>
        public int LimitClassTimes { get; set; }

        /// <summary>
        /// 剩余天数
        /// </summary>
        public int LimitDay { get; set; }

        /// <summary>
        /// 选择的课程
        /// </summary>
        public List<long> MyCourseIds { get; set; }

        /// <summary>
        /// 学员类型   <see cref="ETMS.Entity.Enum.EmStudentType"/>
        /// </summary>
        public byte? StudentType { get; set; }

        /// <summary>
        /// 加载学员信息
        /// </summary>
        public bool IsLoadRich { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(StudentKey))
            {
                condition.Append($" AND (StudentName LIKE '{StudentKey}%' OR StudentPhone LIKE '{StudentKey}%')");
            }
            if (DeType != null)
            {
                condition.Append($" AND DeType = {DeType.Value}");
            }
            if (Status != null)
            {
                condition.Append($" AND Status = {Status.Value}");
            }
            if (CourseId != null)
            {
                condition.Append($" AND CourseId = {CourseId.Value}");
            }
            if (IsQueryShort != null && IsQueryShort.Value)
            {
                condition.Append($" AND BuyQuantity > 0 AND StudentType = {EmStudentType.ReadingStudent} AND ((DeType={EmDeClassTimesType.ClassTimes} AND SurplusQuantity <= {LimitClassTimes}) OR (DeType<>{EmDeClassTimesType.ClassTimes} AND SurplusQuantity=0 AND SurplusSmallQuantity <={LimitDay}))");
            }
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
            if (StudentType != null)
            {
                condition.Append($" AND StudentType = {StudentType.Value}");
            }
            return condition.ToString();
        }
    }
}
