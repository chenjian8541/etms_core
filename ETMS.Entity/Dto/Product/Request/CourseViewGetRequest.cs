using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Request
{
    public class CourseViewGetRequest : RequestBase
    {
        public long CourseId { get; set; }

        public override string Validate()
        {
            if (CourseId <= 0)
            {
                return "请求数据不合法";
            }
            return string.Empty;
        }
    }
}