using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Common
{
    public interface IPagingRequest
    {
        /// <summary>
        /// 每页数
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        int PageCurrent { get; set; }
    }
}
