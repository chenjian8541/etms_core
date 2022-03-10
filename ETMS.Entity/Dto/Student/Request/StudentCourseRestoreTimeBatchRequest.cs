using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCourseRestoreTimeBatchRequest : RequestBase
    {
        public List<long> StudentIds { get; set; }

        public override string Validate()
        {
            if (StudentIds == null || StudentIds.Count == 0)
            {
                return "请选择复课学员";
            }
            if (StudentIds.Count > 50)
            {
                return "一次性最多对50名学员复课";
            }
            return string.Empty;
        }
    }
}
