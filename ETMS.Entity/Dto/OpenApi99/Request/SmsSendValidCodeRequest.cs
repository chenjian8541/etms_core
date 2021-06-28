using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenApi99.Request
{
    public class SmsSendValidCodeRequest : OpenApi99Base
    {
        public string Phone { get; set; }

        public string ValidCode { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Phone))
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(ValidCode))
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
