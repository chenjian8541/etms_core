using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class GiftExchangeLogView
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
        /// 操作人
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 兑换单号
        /// </summary>
        public string No { get; set; }

        public byte ExchangeType { get; set; }

        /// <summary>
        /// 总积分
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 兑换时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmGiftExchangeLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }
    }
}
