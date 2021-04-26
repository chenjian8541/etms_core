using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 套餐详情
    /// </summary>
    [Table("EtSuitDetail")]
    public class EtSuitDetail : Entity<long>
    {
        public long SuitId { get; set; }

        public string Name { get; set; }

        public long CoursePriceRuleId { get; set; }

        /// <summary>
        /// 产品类型  <see cref=" ETMS.Entity.Enum.EmProductType"/>
        /// </summary>
        public byte ProductType { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 定价标准
        /// </summary>
        public string PriceRule { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 购买数量 
        /// </summary>
        public int BuyQuantity { get; set; }

        /// <summary>
        /// 购买单位 <see cref=" ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte BuyUnit { get; set; }

        /// <summary>
        /// 赠送数量
        /// </summary>
        public int GiveQuantity { get; set; }

        /// <summary>
        /// 赠送单位 <see cref=" ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte GiveUnit { get; set; }

        /// <summary>
        /// 单项总金额
        /// </summary>
        public decimal ItemSum { get; set; }

        /// <summary>
        /// 单项折后额
        /// </summary>
        public decimal ItemAptSum { get; set; }

        /// <summary>
        /// 折扣类别  <see cref="ETMS.Entity.Enum.EmDiscountType"/>
        /// </summary>
        public byte DiscountType { get; set; }

        /// <summary>
        /// 折扣值
        /// </summary>
        public decimal DiscountValue { get; set; }
    }
}
