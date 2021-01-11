using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class OrderGetProductInfoOutput
    {
        public List<OrderGetProductInfoCourseItem> OrderCourses { get; set; }

        public List<OrderGetProductInfoGoodsItem> OrderGoods { get; set; }

        public List<OrderGetProductInfoCostItem> OrderCosts { get; set; }
    }

    public class OrderGetProductInfoCourseItem
    {
        public long OrderDetailId { get; set; }

        public string ProductName { get; set; }

        public string PriceRule { get; set; }

        public int BuyQuantity { get; set; }

        public byte BugUnit { get; set; }

        public string BuyQuantityDesc { get; set; }

        public string GiveQuantityDesc { get; set; }

        public string DiscountDesc { get; set; }

        /// <summary>
        /// 剩余数量(课时/天)
        /// </summary>
        public string SurplusQuantity { get; set; }

        public string SurplusQuantityDesc { get; set; }

        /// <summary>
        /// 按月的  以天作为单价
        /// </summary>
        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        public decimal ItemAptSum { get; set; }
        public decimal ItemSum { get; set; }

        /// <summary>
        /// <see cref="OrderGetProductInfoItemsStatus"/>
        /// </summary>
        public int Status { get; set; }
    }

    public class OrderGetProductInfoGoodsItem
    {
        public long OrderDetailId { get; set; }

        public string ProductName { get; set; }

        public string PriceRule { get; set; }

        public int BuyQuantity { get; set; }

        public string BuyQuantityDesc { get; set; }

        public string DiscountDesc { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int SurplusQuantity { get; set; }

        public string SurplusQuantityDesc { get; set; }

        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        /// <summary>
        /// <see cref="OrderGetProductInfoItemsStatus"/>
        /// </summary>
        public int Status { get; set; }

        public decimal ItemAptSum { get; set; }

        public decimal ItemSum { get; set; }
    }

    public class OrderGetProductInfoCostItem
    {
        public long OrderDetailId { get; set; }

        public string ProductName { get; set; }

        public string PriceRule { get; set; }

        public int BuyQuantity { get; set; }

        public string BuyQuantityDesc { get; set; }

        public string DiscountDesc { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int SurplusQuantity { get; set; }

        public string SurplusQuantityDesc { get; set; }

        public decimal Price { get; set; }

        public string PriceDesc { get; set; }

        /// <summary>
        /// <see cref="OrderGetProductInfoItemsStatus"/>
        /// </summary>
        public int Status { get; set; }

        public decimal ItemAptSum { get; set; }

        public decimal ItemSum { get; set; }
    }

    public struct OrderGetProductInfoItemsStatus
    {
        public const byte Normal = 0;

        public const byte Disable = 1;
    }
}
