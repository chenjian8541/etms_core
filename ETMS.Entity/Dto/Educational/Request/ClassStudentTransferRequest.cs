using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassStudentTransferRequest : RequestBase
    {
        public long ClassId { get; set; }

        public long CId { get; set; }

        public long CourseId { get; set; }

        public long NewClassId { get; set; }

        public long StudentId { get; set; }

        public override string Validate()
        {
            if (ClassId <= 0 || CId <= 0 || CourseId <= 0 || NewClassId <= 0 || StudentId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}
