using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Lib
{
    public interface ITenantConfigWrapper
    {
        /// <summary>
        /// 获取机构的数据库连接
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<string> GetTenantConnectionString(int tenantId);

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        Task TenantConnectionUpdate();
    }
}
