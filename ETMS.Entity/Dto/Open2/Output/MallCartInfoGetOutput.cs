using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Output
{
    public class MallCartInfoGetOutput
    {
        public string GId { get; set; }

        public int BuyCount { get; set; }

        public long? CoursePriceRuleId { get; set; }

        public string CoursePriceRuleDesc { get; set; }

        public decimal TotalPrice { get; set; }

        public int TotalPoint { get; set; }

        public List<ParentBuyMallGoodsSubmitSpecItem> SpecItems { get; set; }

        /// <summary>
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmProductType"/>
        /// </summary>
        public byte ProductType { get; set; }

        public string ProductTypeDesc { get; set; }

        public string Name { get; set; }

        public string OriginalPriceDesc { get; set; }

        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        public string ImgCoverUrl { get; set; }

        public long RelatedId { get; set; }
    }
}
