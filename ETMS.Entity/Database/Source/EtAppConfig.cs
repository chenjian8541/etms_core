using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    /// <summary>
    /// 系统配置表
    /// </summary>
    [Table("EtAppConfig")]
    public class EtAppConfig : Entity<long>
    {
        /// <summary>
        /// 系统配置类型  <see cref="ETMS.Entity.Enum.EmAppConfigType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 配置内容(JSON序列化)
        /// </summary>
        public string ConfigValue { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
