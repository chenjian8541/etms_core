using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ETMS.Entity.Dto.Open2.Request
{
    public class CheckPhoneSmsSendRequest : IValidate
    {
        public string Phone { get; set; }

        public string Validate()
        {
            if (string.IsNullOrEmpty(Phone))
            {
                return "请填写手机号码";
            }
            return string.Empty;
        }
    }
}
