using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 套餐
    /// </summary>
    [Table("EtSuit")]
    public class EtSuit : Entity<long>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用 <see cref="ETMS.Entity.Enum.EmProductStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public int SalesCount { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
