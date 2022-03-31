using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Common
{
    public class AlienRequestPagingBase : AlienRequestBase, IPagingRequest
    {
        /// <summary>
        /// 每页数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageCurrent { get; set; }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (PageSize > 0 && PageCurrent > 0)
            {
                return string.Empty;
            }
            else
            {
                return "分页参数错误";
            }
        }

        protected virtual string DataFilterWhereGetLimitOnlyTenant()
        {
            if (this.AllTenants.Count > 1)
            {
                return $"IsDeleted = {EmIsDeleted.Normal} AND TenantId IN ({string.Join(',', AllTenants)})";
            }
            else
            {
                return $"IsDeleted = {EmIsDeleted.Normal} AND TenantId = {AllTenants[0]}";
            }
        }
    }
}
