using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Request
{
    public class ActivityMainCreateInitOfGroupPurchaseRequest : RequestBase
    {
        public long SystemId { get; set; }

        public override string Validate()
        {
            if (SystemId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
