using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class GetIndustryRequrst : IValidate
    {
        public int Type { get; set; }

        public string Code { get; set; }

        public string Validate()
        {
            if (Type < 0 || Type > 2)
            {
                return "请求数据格式错误";
            }
            if (Type > 0 && string.IsNullOrEmpty(Code))
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
