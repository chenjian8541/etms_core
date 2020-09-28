using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common.Request;
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

        public long? ClassCategoryId { get; set; }

        public int DefaultClassTimes { get; set; }

        public List<long> ClassRoomIds { get; set; }

        public List<MultiSelectValueRequest> TeacherIds { get; set; }

        public string Remark { get; set; }

        public bool IsLeaveCharge { get; set; }

        public bool IsNotComeCharge { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据不合法";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入班级名称";
            }
            return base.Validate();
        }
    }
}