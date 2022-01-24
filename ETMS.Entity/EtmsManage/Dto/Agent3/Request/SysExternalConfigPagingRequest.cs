using ETMS.Entity.Enum;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Text;


namespace ETMS.Entity.EtmsManage.Dto.Agent3.Request
{
    public class SysExternalConfigPagingRequest : AgentPagingBase
    {
        public string Type { get; set; }

        public string Name { get; set; }

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
            var condition = new StringBuilder();
            condition.Append($"IsDeleted = {EmIsDeleted.Normal}");
            if (!string.IsNullOrEmpty(Type))
            {
                condition.Append($" AND [Type] = '{Type}'");
            }
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND [Name] LIKE '%{Name}%'");
            }
            return condition.ToString();
        }
    }
}
