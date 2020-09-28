using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Entity.Dto.Common.Request;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassAddRequest : RequestBase
    {
        public string Name { get; set; }

        public List<MultiSelectValueRequest> CourseIds { get; set; }

        public int? LimitStudentNums { get; set; }

        public long? ClassCategoryId { get; set; }

        public int DefaultClassTimes { get; set; }

        public List<long> ClassRoomIds { get; set; }

        public List<MultiSelectValueRequest> TeacherIds { get; set; }

        public string Remark { get; set; }

        public bool IsLeaveCharge { get; set; }

        public bool IsNotComeCharge { get; set; }

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
            return base.Validate();
        }
    }
}
