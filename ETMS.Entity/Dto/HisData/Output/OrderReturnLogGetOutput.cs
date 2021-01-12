using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class OrderReturnLogGetOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public int OrderType { get; set; }

        /// <summary>
        /// 支出类型 <see cref="ETMS.Entity.Enum.EmOrderInOutType"/>
        /// </summary>
        public byte InOutType { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string No { get; set; }

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
        /// 状态 <see cref="ETMS.Entity.Enum.EmOrderStatus"/>
        /// </summary>
        public byte Status { get; set; }

        public string StatusDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public List<OrderReturnLogDetail> LogDetails { get; set; }
    }

    public class OrderReturnLogDetail {

        public long CId { get; set; }

        public string ProductTypeDesc { get; set; }

        public string ProductName { get; set; }

        public string OutQuantity { get; set; }

        public decimal ItemSum { get; set; }

        public decimal ItemAptSum { get; set; }
    }
}
