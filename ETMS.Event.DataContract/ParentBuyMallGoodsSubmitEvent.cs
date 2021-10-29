using ETMS.Entity.CacheBucket.Mall;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class ParentBuyMallGoodsSubmitEvent : Event
    {
        public ParentBuyMallGoodsSubmitEvent(int tenantId) : base(tenantId)
        {
        }

        public EtMallOrder MallOrder { get; set; }

        public MallGoodsBucket MyMallGoodsBucket { get; set; }

        public EtTenantLcsPayLog MyTenantLcsPayLog { get; set; }

        public EtStudent MyStudent { get; set; }

        public EtMallCoursePriceRule CoursePriceRule { get; set; }
    }
}
