using ETMS.Entity.Database.Manage;
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

        Task<TencentCloudAccountView> GetTencentCloudAccount(int tencentCloudId);

        Task<SysCustomerServiceInfo> GetDefalutCustomerServiceInfo();
    }
}
