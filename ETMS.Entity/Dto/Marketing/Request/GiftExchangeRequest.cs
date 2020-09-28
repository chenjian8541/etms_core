using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class GiftExchangeRequest : ParentRequestBase
    {
        public List<ExchangeGiftInfo> ExchangeGiftInfos { get; set; }

        public long StudentId { get; set; }

        public int TotalPoint { get; set; }

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

    public class ExchangeGiftInfo
    {
        public long GiftId { get; set; }

        public int Count { get; set; }
    }
}
