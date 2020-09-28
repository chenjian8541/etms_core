using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesCancelTryClassStudentRequest : RequestBase
    {
        public long ClassTimesId { get; set; }
        public long StudentTryCalssLogId { get; set; }

        public string StudentName { get; set; }
        public override string Validate()
        {
            if (ClassTimesId <= 0 || StudentTryCalssLogId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}
