using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 系统全局配置
    /// </summary>
    [Table("SysAppsettings")]
    public class SysAppsettings : EManageEntity<int>
    {
        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmSysAppsettingsType"/>
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }
    }
}
