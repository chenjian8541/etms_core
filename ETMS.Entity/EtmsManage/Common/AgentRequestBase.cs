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

        public long LoginUserId { get; set; }

        public bool LoginAgentIsLimitData { get; set; }

        public bool LoginUserIsLimitData { get; set; }

        protected virtual string DataFilterWhereGet(string agentIdTag = "AgentId")
        {
            var sql = new StringBuilder();
            if (LoginAgentIsLimitData)
            {
                sql.Append($"{agentIdTag} = {LoginAgentId} AND IsDeleted = {EmIsDeleted.Normal}");
            }
            else
            {
                sql.Append($"IsDeleted = {EmIsDeleted.Normal}");
            }
            if (LoginUserIsLimitData && IsNeedLimitUserData())
            {
                sql.Append($" AND UserId = {LoginUserId} ");
            }
            return sql.ToString();
        }

        /// <summary>
        /// 数据较验
        /// </summary>
        /// <returns></returns>
        public virtual string Validate()
        {
            return string.Empty;
        }

        /// <summary>
        /// 是否需要限制用户数据
        /// </summary>
        /// <returns></returns>
        public virtual bool IsNeedLimitUserData()
        {
            return false;
        }
    }
}
