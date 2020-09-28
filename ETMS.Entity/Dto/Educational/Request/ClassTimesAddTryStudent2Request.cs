using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesAddTryStudent2Request : RequestBase
    {
        public long CId { get; set; }

        public long StudentId { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据不合法";
            }
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            return string.Empty;
        }
    }
}