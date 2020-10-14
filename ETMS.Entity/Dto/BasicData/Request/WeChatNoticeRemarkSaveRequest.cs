using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class WeChatNoticeRemarkSaveRequest: RequestBase
    {
        /// <summary>
        /// 微信推送后缀
        /// </summary>
        public string WeChatNoticeRemark { get; set; }
    }
}
