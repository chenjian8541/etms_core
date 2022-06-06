using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Entity.Dto.Common.Request;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassAddRequest : RequestBase
    {
        public string Name { get; set; }

        public List<MultiSelectValueRequest> CourseIds { get; set; }

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
        public override string Validate()
        {
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
            if (TeacherIds != null && TeacherIds.Count > 10)
            {
                return "最多设置10位上课老师";
            }
            return base.Validate();
        }
    }
}
