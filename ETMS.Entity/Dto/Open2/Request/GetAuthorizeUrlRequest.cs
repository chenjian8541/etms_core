using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class GetAuthorizeUrlRequest : Open2Base
    {
        public string SourceUrl { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SourceUrl))
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}

