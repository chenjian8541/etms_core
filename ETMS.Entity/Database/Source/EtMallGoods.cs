using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtMallGoods")]
    public class EtMallGoods : Entity<long>
    {
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

        public decimal OriginalPrice { get; set; }

        public string OriginalPriceDesc { get; set; }

        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        public string TagContent { get; set; }

        public string SpecContent { get; set; }

        public string ImgCover { get; set; }

        public string ImgDetail { get; set; }

        public string GsContent { get; set; }
    }
}
