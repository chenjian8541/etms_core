using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Config;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysAppsettingsBLL
    {
        Task<SysTenantWechartAuth> GetWechartAuthDefault();

        [Obsolete("使用SysAITenantAccount,SysAIFaceBiduAccount库存储云账户")]
        Task<TencentCloudAccountView> GetTencentCloudAccount(int tencentCloudId);

        Task<EtmsGlobalConfig> GetEtmsGlobalConfig();
    }
}
