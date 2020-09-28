using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 优惠券核销记录
    /// </summary>
    [Table("EtCouponsStudentUse")]
    public class EtCouponsStudentUse : Entity<long>
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
        /// 订单ID
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 核销时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
