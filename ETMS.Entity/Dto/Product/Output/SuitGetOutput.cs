using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Product.Output
{
    public class SuitGetOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 赠送积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public int SalesCount { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public List<SuitDetailCourseItem> SuitCourse { get; set; }

        public List<SuitDetailGoodsItem> SuitGoods { get; set; }

        public List<SuitDetailCostItem> SuitCost { get; set; }
    }

    public class SuitDetailCourseItem
    {
        public CourseGetPagingOutput MyData { get; set; }

        public long MyCourseId { get; set; }

        public string Name { get; set; }

        public List<PriceRuleDesc> PriceRuleDescs { get; set; }

        public int Quantity { get; set; }

        public int MyBuyQuantity { get; set; }

        public string PriceTypeDesc { get; set; }

        public byte PriceType { get; set; }

        public byte MyBuyUnit { get; set; }

        public string MyGiveQuantity { get; set; }

        public decimal ItemTotalSum { get; set; }

        public byte MyGiveUnit { get; set; }

        public string MyDiscountValue { get; set; }

        public decimal MyDiscountType { get; set; }

        public decimal MyItemAptSum { get; set; }

        public long MyCoursePriceRuleId { get; set; }

        public decimal TotalPrice { get; set; }

        public int Points { get; set; }
    }

    public class SuitDetailGoodsItem
    {
        public GoodsGetPagingOutput MyData { get; set; }

        public long MyGoodsId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public int MyBuyQuantity { get; set; }

        public decimal ItemTotalSum { get; set; }

        public string MyDiscountValue { get; set; }

        public byte MyDiscountType { get; set; }

        public decimal MyItemAptSum { get; set; }

        public decimal Price { get; set; }

        public int Points { get; set; }
    }

    public class SuitDetailCostItem {
        public CostGetPagingOutput MyData { get; set; }

        public long MyCostId { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public int MyBuyQuantity { get; set; }

        public decimal ItemTotalSum { get; set; }

        public string MyDiscountValue { get; set; }

        public byte MyDiscountType { get; set; }

        public decimal MyItemAptSum { get; set; }

        public decimal Price { get; set; }

        public int Points { get; set; }
    }
}
