using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.SysCommon.Request
{
    public class EtmsGlobalConfigSaveRequest: AgentRequestBase
    {
        /// <summary>
        /// 客服QQ
        /// </summary>
        public string KefuQQ { get; set; }

        /// <summary>
        /// 客服手机号码
        /// </summary>
        public string KefuPhone { get; set; }

        /// <summary>
        /// 404电话
        /// </summary>
        public string Phone404 { get; set; }

        /// <summary>
        /// 53客服
        /// </summary>
        public string Kefu53 { get; set; }
    }
}
