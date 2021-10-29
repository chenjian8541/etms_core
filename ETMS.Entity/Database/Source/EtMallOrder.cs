using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtMallOrder")]
    public class EtMallOrder : Entity<long>
    {
        public long StudentId { get; set; }

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

        public string ImgCover { get; set; }

        public string PriceRuleDesc { get; set; }

        public string GoodsSpecContent { get; set; }

        public long LcsPayLogId { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime CreateOt { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMallOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string Remark { get; set; }
    }
}
