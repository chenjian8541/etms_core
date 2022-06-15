using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassEditRequest : RequestBase
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public int? LimitStudentNums { get; set; }

        /// <summary>
        /// 班级容量类型
        /// <see cref="ETMS.Entity.Enum.EmLimitStudentNumsType"/>
        /// </summary>
        public byte LimitStudentNumsType { get; set; }

        public long? ClassCategoryId { get; set; }

        public decimal DefaultClassTimes { get; set; }

        public List<long> ClassRoomIds { get; set; }

        public List<MultiSelectValueRequest> TeacherIds { get; set; }

        public string Remark { get; set; }

        public bool IsLeaveCharge { get; set; }

        public bool IsNotComeCharge { get; set; }

        public bool IsCanOnlineSelClass { get; set; }
        public List<MultiSelectValueRequest> CourseIds { get; set; }

        /// <summary>
        /// 一对一约课类型 <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReservationType { get; set; }

        public int DurationHour { get; set; }

        public int DurationMinute { get; set; }

        public int DunIntervalMinute { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入班级名称";
            }
            if (CourseIds == null || !CourseIds.Any())
            {
                return "请选择关联课程";
            }
            if (CourseIds.Count > 50)
            {
                return "最多设置50门关联的课程";
            }
            if (LimitStudentNumsType == EmLimitStudentNumsType.NotOverflow)
            {
                if (LimitStudentNums == null || LimitStudentNums <= 0)
                {
                    return "请设置班级容量";
                }
            }
            if (TeacherIds != null && TeacherIds.Count > 20)
            {
                return "最多设置20位上课老师";
            }
            return base.Validate();
        }
    }
}