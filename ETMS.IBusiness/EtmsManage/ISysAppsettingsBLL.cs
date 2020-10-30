using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysAppsettingsBLL
    {
        Task<SysTenantWechartAuth> GetWechartAuthDefault();
    }
}
