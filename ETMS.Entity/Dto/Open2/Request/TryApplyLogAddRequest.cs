using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class TryApplyLogAddRequest : OpenTouristBase
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string SmsCode { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "请填写机构名称";
            }
            if (string.IsNullOrEmpty(Phone))
            {
                return "请填写手机号码";
            }
            if (string.IsNullOrEmpty(SmsCode))
            {
                return "验证码不能为空";
            }
            return string.Empty;
        }
    }
}
