using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 用户操作日志
    /// </summary>
    [Table("EtUserOperationLog")]
    public class EtUserOperationLog : Entity<long>
    {
        /// <summary>
        /// 操作用户
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 操作类型  <see cref="ETMS.Entity.Enum.EmUserOperationType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string OpContent { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
