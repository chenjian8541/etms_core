﻿using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IDataAccess.Lcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Lcs
{
    public class TenantLcsPayLogDAL : DataAccessBase, ITenantLcsPayLogDAL
    {
        public TenantLcsPayLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<long> AddTenantLcsPayLog(EtTenantLcsPayLog entity)
        {
            await this._dbWrapper.Insert(entity);
            return entity.Id;
        }

        public async Task<EtTenantLcsPayLog> GetTenantLcsPayLog(long id)
        {
            return await this._dbWrapper.Find<EtTenantLcsPayLog>(p => p.Id == id);
        }

        public async Task EditTenantLcsPayLog(EtTenantLcsPayLog entity)
        {
            await this._dbWrapper.Update(entity);
        }

        public async Task<Tuple<IEnumerable<EtTenantLcsPayLog>, int>> GetTenantLcsPayLogPaging(IPagingRequest request)
        {
            return await this._dbWrapper.ExecutePage<EtTenantLcsPayLog>("EtTenantLcsPayLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
