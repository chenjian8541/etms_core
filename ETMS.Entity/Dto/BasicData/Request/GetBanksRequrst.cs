using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class GetBanksRequrst : IValidate
    {
        public int Type { get; set; }

        public string BankCode { get; set; }

        public string CityCode { get; set; }

        public string Validate()
        {
            if (Type < 0 || Type > 1)
            {
                return "请求数据格式错误";
            }
            if (Type == 1)
            {
                if (string.IsNullOrEmpty(BankCode) || string.IsNullOrEmpty(CityCode))
                {
                    return "请求数据格式错误";
                }
            }
            return string.Empty;
        }
    }
}
