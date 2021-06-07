using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 主数据库共有字段
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    [Serializable]
    public abstract class Entity<TPrimaryKey>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual TPrimaryKey Id { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        public virtual int TenantId { get; set; }

        /// <summary>
        /// 是否删除  <see cref=" ETMS.Entity.Enum.EmIsDeleted"/>
        /// </summary>
        public virtual byte IsDeleted { get; set; }
    }
}
