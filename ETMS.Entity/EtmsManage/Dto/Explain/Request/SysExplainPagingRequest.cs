using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Explain.Request
{
    public class SysExplainPagingRequest : AgentPagingBase
    {
        public int? Type { get; set; }

        /// <summary>
        /// 是否需要限制用户数据
        /// </summary>
        /// <returns></returns>
        public override bool IsNeedLimitUserData()
        {
            return true;
        }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet("AgentId"));
            if (Type != null)
            {
                condition.Append($" AND [Type] = {Type}");
            }
            return condition.ToString();
        }
    }
}
