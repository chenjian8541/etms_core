using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent5.Request
{
    public class ActivityMainGetPagingRequest: ParentRequestPagingBase
    {
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder($"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal}");
            condition.Append($" AND IsShowInParent = 1 AND ActivityStatus = {EmActivityStatus.Processing}");
            return condition.ToString();
        }
    }
}
