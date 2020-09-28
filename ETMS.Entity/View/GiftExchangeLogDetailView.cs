using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class GiftExchangeLogDetailView
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
        /// 礼品ID
        /// </summary>
        public long GiftId { get; set; }

        /// <summary>
        /// 兑换ID
        /// </summary>
        public long GiftExchangeLogId { get; set; }

        /// <summary>
        /// 兑换单号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmGiftExchangeLogStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 消耗积分
        /// </summary>
        public int ItemPoints { get; set; }

        public string GiftName { get; set; }

        public string GiftImgPath { get; set; }

        /// <summary>
        /// 兑换时间
        /// </summary>
        public DateTime Ot { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }
    }
}
