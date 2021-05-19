using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentPagingRequest : AgentPagingBase
    {
        public string Key { get; set; }

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
            var condition = new StringBuilder(DataFilterWhereGet("Id"));
            if (!string.IsNullOrEmpty(Key))
            {
                condition.Append($" AND (Name LIKE '{Key}%' OR Phone LIKE '{Key}%' )");
            }
            return condition.ToString();
        }
    }
}
