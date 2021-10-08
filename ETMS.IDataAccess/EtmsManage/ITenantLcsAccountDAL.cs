﻿using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ITenantLcsAccountDAL
    {
        Task<SysTenantLcsAccount> GetTenantLcsAccount(long tenantId);

        Task<SysTenantLcsAccount> GetTenantLcsAccount(string merchantNo);

        Task AddTenantLcsAccount(SysTenantLcsAccount entity);

        Task EditTenantLcsAccount(SysTenantLcsAccount entity);

        Task<long> AddTenantLcsPayLog(SysTenantLcsPayLog entity);

        Task<SysTenantLcsPayLog> GetTenantLcsPayLog(long id);

        Task EditTenantLcsPayLog(SysTenantLcsPayLog entity);

        Task<Tuple<IEnumerable<SysTenantLcsPayLog>, int>> GetTenantLcsPayLogPaging(IPagingRequest request);
    }
}
