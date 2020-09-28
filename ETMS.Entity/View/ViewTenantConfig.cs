using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    /// <summary>
    /// 机构信息
    /// </summary>
    public class ViewTenantConfig
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 数据库配置
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
