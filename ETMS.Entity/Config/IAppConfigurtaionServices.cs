using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    /// <summary>
    /// 应用程序配置管理
    /// </summary>
    public interface IAppConfigurtaionServices
    {
        /// <summary>
        /// AppSettings配置
        /// </summary>
        AppSettings AppSettings { get; set; }
    }
}
