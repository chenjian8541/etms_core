using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class CouponsStudentUseView
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

        public string CouponsTitle { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public byte CouponsType { get; set; }

        public decimal CouponsValue { get; set; }
    }
}
