using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class TenantSimpleInfoGetMIRequest : IValidate
    {
        public string TenantNo { get; set; }

        public virtual string Validate()
        {
            if (string.IsNullOrEmpty(TenantNo))
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}