using ETMS.Entity.Dto.Common;
using ETMS.Entity.View.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Product.Output
{
    public class MallGoodsGetOutput
    {
        public long Id { get; set; }

        public string GId { get; set; }

        /// <summary>
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmProductType"/>
        /// </summary>
        public byte ProductType { get; set; }

        public string ProductTypeDesc { get; set; }

        public long RelatedId { get; set; }

        public string RelatedName { get; set; }

        public string Name { get; set; }

        public decimal OriginalPrice { get; set; }

        public string OriginalPriceDesc { get; set; }

        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        public int Points { get; set; }

        public List<MallGoodsTagItem> TagItems { get; set; }

        public List<MallGoodsSpecItem> SpecItems { get; set; }

        public string imgCoverKey { get; set; }

        public string imgCoverUrl { get; set; }

        public List<Img> ImgDetail { get; set; }

        public string GsContent { get; set; }

        public CoursePriceRuleOutput CoursePriceRules { get; set; }
    }
}
