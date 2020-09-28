using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    /// <summary>
    /// 缓存数据约定
    /// </summary>
    public interface ICacheDataContract
    {
        /// <summary>
        /// 获取缓存key
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        string GetKeyFormat(params object[] parms);

        /// <summary>
        /// 缓存过期时间
        /// </summary>
        TimeSpan TimeOut { get; }
    }
}
