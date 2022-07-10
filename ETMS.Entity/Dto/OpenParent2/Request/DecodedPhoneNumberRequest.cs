using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Request
{
    public class DecodedPhoneNumberRequest : OpenParent2RequestBase
    {
        public string EncryptedData { get; set; }

        public string Iv { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(EncryptedData) || string.IsNullOrEmpty(Iv))
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
