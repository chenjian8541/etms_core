using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysTenantOperationLogDAL : ISysTenantOperationLogDAL, IEtmsManage
    {
        public async Task AddSysTenantOperationLog(SysTenantOperationLog entity)
        {
            await this.Insert(entity);
        }

        public async Task AddSysTenantOperationLog(List<SysTenantOperationLog> entitys)
        {
            await this.InsertRange(entitys);
        }

        public async Task<Tuple<IEnumerable<SysTenantOperationLog>, int>> GetPaging(IPagingRequest request)
        {
            return await this.ExecutePage<SysTenantOperationLog>("SysTenantOperationLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
