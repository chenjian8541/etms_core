using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class SysUpgradeMsgPagingRequest : AgentPagingBase
    {
        /// <summary>
        /// 代理商信息
        /// </summary>
        public string VersionNo { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet());
            if (!string.IsNullOrEmpty(VersionNo))
            {
                condition.Append($" AND VersionNo = {VersionNo}");
            }
            return condition.ToString();
        }
    }
}
