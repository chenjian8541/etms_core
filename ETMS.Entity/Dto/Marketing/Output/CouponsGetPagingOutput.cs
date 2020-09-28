using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Output
{
    public class CouponsGetPagingOutput
    {
        public long CId { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmCouponsType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        /// <summary>
        /// 优惠额度
        /// </summary>
        public decimal MyValue { get; set; }

        public string ValueDesc { get; set; }

        /// <summary>
        /// 优惠券标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 最低消费
        /// </summary>
        public decimal MinLimit { get; set; }

        public string MinLimitDesc { get; set; }

        /// <summary>
        /// 总量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 领券数量
        /// </summary>
        public int GetCount { get; set; }

        /// <summary>
        /// 核销数量
        /// </summary>
        public int UsedCount { get; set; }

        /// <summary>
        /// 使用说明
        /// </summary>
        public string UseExplain { get; set; }

        /// <summary>
        /// 过期类型   <see cref="ETMS.Entity.Enum.EmCouponsExpiredType"/>
        /// </summary>
        public byte ExpiredType { get; set; }

        public string ExpiredDesc { get; set; }

        /// <summary>
        /// 单人单日限领
        /// 0表示不限制
        /// </summary>
        public int LimitGetSingle { get; set; }

        public string LimitGetSingleDesc { get; set; }

        /// <summary>
        /// 单人限领
        /// 0表示不限制
        /// </summary>
        public int LimitGetAll { get; set; }

        public string LimitGetAllDesc { get; set; }

        /// <summary>
        /// 状态  <see cref="ETMS.Entity.Enum.EmCouponsStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public long Value { get; set; }

        public string Label { get; set; }

        public bool IsExpired { get; set; }
    }
}
