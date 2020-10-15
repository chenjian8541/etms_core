using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Common
{
    public class AgentRequestBase : IValidate
    {
        public int LoginAgentId { get; set; }

        public bool LoginIsLimitData { get; set; }

        protected virtual string DataFilterWhereGet(string agentIdTag = "AgentId")
        {
            if (LoginIsLimitData)
            {
                return $"{agentIdTag} = {LoginAgentId} AND IsDeleted = {EmIsDeleted.Normal}";
            }
            else
            {
                return $"IsDeleted = {EmIsDeleted.Normal}";
            }
        }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public virtual string Validate()
        {
            return string.Empty;
        }
    }
}
