using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class GetWxOpenIdRequest : Open2Base
    {
        public string Code { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Code))
            {
                return "授权code不能为空";
            }
            return string.Empty;
        }
    }
}
