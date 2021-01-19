using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    [Table("SysConnectionString")]
    public class SysConnectionString : EManageEntity<int>
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int ConIndex { get; set; }

        /// <summary>
        /// 组
        /// </summary>
        public int ConGroup { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EtmsManage.EmSysConnectionStringStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
