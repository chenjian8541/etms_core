using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class GiftExchangeReceptionRequest : RequestBase
    {
        public long StudentId { get; set; }

        public List<GiftExchangeReceptionItem> ExchangeGiftInfos { get; set; }

        public long TotalPoint { get; set; }

        public string Remark { get; set; }
        public override string Validate()
        {
            if (ExchangeGiftInfos == null || !ExchangeGiftInfos.Any())
            {
                return "请选择要兑换的礼品";
            }
            return base.Validate();
        }
    }

    public class GiftExchangeReceptionItem
    {
        public long GiftId { get; set; }

        public int Count { get; set; }
    }
}
