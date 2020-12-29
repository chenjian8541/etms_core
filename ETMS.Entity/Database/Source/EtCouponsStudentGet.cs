using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 学员优惠券
    /// </summary>
    [Table("EtCouponsStudentGet")]
    public class EtCouponsStudentGet : Entity<long>
    {
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
        /// 生成的批次单号
        /// </summary>
        public string GenerateNo { get; set; }

        /// <summary>
        /// 是否已发送提醒 <see cref=" ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsRemindExpired { get; set; }

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
    }
}
