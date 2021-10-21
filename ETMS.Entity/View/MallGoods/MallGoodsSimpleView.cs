using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View.MallGoods
{
    public class MallGoodsSimpleView
    {
        public long Id { get; set; }

        /// <summary>
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmProductType"/>
        /// </summary>
        public byte ProductType { get; set; }

        public string ProductTypeDesc { get; set; }

        public long RelatedId { get; set; }

        public string Name { get; set; }

        public long OrderIndex { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        public string ImgCover { get; set; }

        public string RelatedName { get; set; }
    }
}
