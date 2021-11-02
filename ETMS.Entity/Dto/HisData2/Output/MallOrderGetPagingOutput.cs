using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.HisData2.Output
{
    public class MallOrderGetPagingOutput
    {
        public long CId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public long MallGoodsId { get; set; }

        public string OrderNo { get; set; }

        /// <summary>
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmProductType"/>
        /// </summary>
        public byte ProductType { get; set; }

        public string ProductTypeDesc { get; set; }

        public long RelatedId { get; set; }

        public int BuyCount { get; set; }

        public decimal AptSum { get; set; }

        public int TotalPoints { get; set; }

        public decimal PaySum { get; set; }

        public string GoodsName { get; set; }

        public List<ParentBuyMallGoodsSubmitSpecItem> GoodsSpecContent { get; set; }

        public string ImgCoverUrl { get; set; }

        public string PriceRuleDesc { get; set; }

        public long LcsPayLogId { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime CreateOt { get; set; }

        public long? OrderId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMallOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        public string Remark { get; set; }
    }
}
