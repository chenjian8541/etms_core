using ETMS.DataAccess.Core;
using ETMS.Entity.Database.Manage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantWechartErrorDAL : ISysTenantWechartErrorDAL, IEtmsManage
    {
        public async Task<bool> AddSysTenantWechartError(SysTenantWechartError entity)
        {
            await this.Insert(entity);
            return true;
        }
    }
}
