using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Request
{
    public class GoodsInventoryLogAddRequest : RequestBase
    {
        public long GoodsId { get; set; }

        public int Quantity { get; set; }

        public decimal Prince { get; set; }

        public decimal TotalMoney { get; set; }

        public string Remark { get; set; }

        public DateTime Ot { get; set; }

        public override string Validate()
        {
            if (GoodsId <= 0)
            {
                return "请求数据格式错误";
            }
            if (Quantity <= 0)
            {
                return "入库数量必须大于0";
            }
            return base.Validate();
        }
    }
}
