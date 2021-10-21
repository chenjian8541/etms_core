using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent3.Request
{
    public class ParentBuyMallGoodsPrepayRequest : ParentRequestBase
    {
        public long GId { get; set; }

        public int BuyCount { get; set; }

        public long? CoursePriceRuleId { get; set; }

        public string OpenId { get; set; }

        public override string Validate()
        {
            if (GId <= 0)
            {
                return "请求数据格式错误";
            }
            if (BuyCount <= 0)
            {
                return "购买数量必须大于0";
            }
            return base.Validate();
        }
    }
}
