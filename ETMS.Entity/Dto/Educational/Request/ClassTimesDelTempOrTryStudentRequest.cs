using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesDelTempOrTryStudentRequest : RequestBase
    {
        public long ClassTimesId { get; set; }
        public long ClassTimesStudentId { get; set; }

        public string StudentName { get; set; }
        public override string Validate()
        {
            if (ClassTimesId <= 0 || ClassTimesStudentId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
