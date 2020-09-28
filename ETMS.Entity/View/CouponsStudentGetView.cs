using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class CouponsStudentGetView
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 是否删除  <see cref=" ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public byte IsDeleted { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        /// <summary>
        /// 优惠券ID
        /// </summary>
        public long CouponsId { get; set; }

        /// <summary>
        /// 允许使用开始时间
        /// 为空则不限制
        /// </summary>
        public DateTime? LimitUseTime { get; set; }

        /// <summary>
        /// 过期时间
        /// 为空则不过期
        /// </summary>
        public DateTime? ExpiredTime { get; set; }

        /// <summary>
        /// 获取时间
        /// </summary>
        public DateTime GetTime { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmCouponsStudentStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string CouponsTitle { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmCouponsType"/>
        /// </summary>
        public byte CouponsType { get; set; }

        public decimal CouponsValue { get; set; }

        public decimal? CouponsMinLimit { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmCouponsStatus"/>
        /// </summary>
        public byte CouponsStatus { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }
    }
}
