using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    /// <summary>
    /// 请求上下文内容
    /// 使用共享同一个请求上下文的方式来传递数据
    /// </summary>
    public class EtmsRequestContent
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; } = 5;
    }
}
