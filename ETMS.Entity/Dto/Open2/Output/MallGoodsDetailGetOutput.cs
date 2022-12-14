using ETMS.Entity.Dto.Product.Output;
using ETMS.Entity.View.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open2.Output
{
    public class MallGoodsDetailGetOutput
    {
        public string GId { get; set; }

        /// <summary>
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmProductType"/>
        /// </summary>
        public byte ProductType { get; set; }

        public string ProductTypeDesc { get; set; }

        public string Name { get; set; }

        public string OriginalPriceDesc { get; set; }

        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        public int Points { get; set; }
        public List<MallGoodsTagItem> TagItems { get; set; }

        public List<MallGoodsSpecItem> SpecItems { get; set; }

        public long RelatedId { get; set; }

        public string ImgCoverUrl { get; set; }

        public string GsContent { get; set; }

        public List<PriceRuleDesc> CoursePriceRules { get; set; }
    }
}
