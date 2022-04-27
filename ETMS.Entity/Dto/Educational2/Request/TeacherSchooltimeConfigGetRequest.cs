using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational2.Request
{
    public class TeacherSchooltimeConfigGetRequest : RequestBase
    {
        public long TeacherId { get; set; }

        public override string Validate()
        {
            if (TeacherId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
