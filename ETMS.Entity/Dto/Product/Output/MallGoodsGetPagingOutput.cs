using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Product.Output
{
    public class MallGoodsGetPagingOutput
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

        public long OrderIndex { get; set; }

        public string OriginalPriceDesc { get; set; }

        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        public string ImgCoverUrl { get; set; }

        public List<PriceRuleDesc> PriceRuleDescs { get; set; }

        public bool IsCanUp { get; set; }

        public bool IsCanDown { get; set; }

        public bool IsLoading { get; set; }
    }
}
