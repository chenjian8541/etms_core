using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenApi99.Request
{
    public class SmsSendRequest : OpenApi99Base
    {
        public string SmsContent { get; set; }

        public List<string> Phones { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SmsContent))
            {
                return "请求数据格式错误";
            }
            if (Phones == null || Phones.Count == 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
