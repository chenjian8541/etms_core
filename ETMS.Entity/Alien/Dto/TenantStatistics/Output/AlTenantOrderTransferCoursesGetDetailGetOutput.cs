
using ETMS.Entity.Dto.Common.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Output
{
    public class AlTenantOrderTransferCoursesGetDetailGetOutput
    {
        public AlTenantOrderTransferCoursesGetDetailBascInfo BascInfo { get; set; }

        public List<AlTenantOrderTransferCoursesGetDetailOut> OutList { get; set; }

        public List<AlTenantOrderTransferCoursesGetDetailIn> InList { get; set; }

        public List<AlTenantOrderGetDetailIncomeLog> OrderGetDetailIncomeLogs { get; set; }

        public decimal OutSum { get; set; }

        public decimal InSum { get; set; }
    }

    public class AlTenantOrderTransferCoursesGetDetailBascInfo
    {
        public long CId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public string StudentAvatar { get; set; }

        public int OrderType { get; set; }

        public string OrderTypeDesc { get; set; }

        /// <summary>
        /// 支出类型 <see cref="ETMS.Entity.Enum.EmOrderInOutType"/>
        /// </summary>
        public byte InOutType { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 购买的课程以”,”隔开
        /// </summary>
        public string BuyCourse { get; set; }

        /// <summary>
        /// 购买的商品
        /// </summary>
        public string BuyGoods { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public string BuyCost { get; set; }

        public string BuyOther { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal Sum { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal AptSum { get; set; }

        /// <summary>
        /// 获得积分
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public string TotalPointsDesc { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PaySum { get; set; }

        /// <summary>
        /// 欠费金额
        /// </summary>
        public decimal ArrearsSum { get; set; }

        /// <summary>
        /// 经办人
        /// </summary>
        public long UserId { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public string OtDesc { get; set; }

        public DateTime CreateOt { get; set; }

        /// <summary>
        /// 业绩归属人
        /// </summary>
        public string CommissionUser { get; set; }

        public string CommissionUserDesc { get; set; }

        public List<MultiSelectValueRequest> CommissionUserIds { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EmOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string UnionOrderId { get; set; }

        public string UnionOrderNo { get; set; }

        public byte IsReturn { get; set; }

        public byte IsTransferCourse { get; set; }
    }

    public class AlTenantOrderTransferCoursesGetDetailOut
    {
        public long CId { get; set; }

        public long UnionOrderId { get; set; }

        public string UnionOrderNo { get; set; }

        public string ProductName { get; set; }

        public string OutQuantity { get; set; }

        public string OutQuantityDesc { get; set; }

        public decimal ItemAptSum { get; set; }
    }

    public class AlTenantOrderTransferCoursesGetDetailIn
    {
        public long CId { get; set; }

        public string ProductTypeDesc { get; set; }

        public string ProductName { get; set; }

        public string PriceRule { get; set; }

        public int BuyQuantity { get; set; }

        public byte BugUnit { get; set; }

        public string BuyQuantityDesc { get; set; }

        public string GiveQuantityDesc { get; set; }

        public decimal ItemSum { get; set; }

        public decimal ItemAptSum { get; set; }

        public string DiscountDesc { get; set; }

        /// <summary>
        /// 退货数量/或者转课数量
        /// </summary>
        public decimal OutQuantity { get; set; }

        public string OutQuantityDesc { get; set; }
    }
}
