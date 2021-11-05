using ETMS.Entity.Common;
using ETMS.Entity.View;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class MallCartAddRequest : Open2Base
    {
        public string GId { get; set; }

        public long Id { get; set; }

        public int BuyCount { get; set; }

        public long? CoursePriceRuleId { get; set; }

        public string CoursePriceRuleDesc { get; set; }

        public decimal TotalPrice { get; set; }

        public int TotalPoint { get; set; }

        public List<ParentBuyMallGoodsSubmitSpecItem> SpecItems { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(GId))
            {
                return "请求数据格式错误";
            }
            Id = EtmsHelper2.GetIdDecrypt2(GId);
            if (Id <= 0)
            {
                return "请求数据格式错误";
            }
            if (BuyCount <= 0)
            {
                return "购买数量不能小于0";
            }
            if (TotalPrice <= 0)
            {
                return "支付金额必须大于0";
            }
            return base.Validate();
        }
    }
}
