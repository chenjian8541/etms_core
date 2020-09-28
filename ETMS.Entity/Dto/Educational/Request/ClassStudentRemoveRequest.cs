using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Entity.Common;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassStudentRemoveRequest : RequestBase
    {
        public long ClassId { get; set; }

        public long CId { get; set; }

        public override string Validate()
        {
            if (ClassId <= 0 || CId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}
