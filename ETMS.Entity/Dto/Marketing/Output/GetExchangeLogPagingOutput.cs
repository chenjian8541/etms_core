using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Output
{
    public class GetExchangeLogPagingOutput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long CId { get; set; }

        /// <summary>
        /// 兑换单号
        /// </summary>
        public string No { get; set; }

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

        public string StatusDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public byte ExchangeType { get; set; }

        public string ExchangeTypeDesc { get; set; }
    }
}
