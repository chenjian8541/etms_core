using ETMS.DataAccess.Core;
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

        public async Task<EtTenantLcsPayLog> GetTenantLcsPayLogBuyOutTradeNo(string outTradeNo)
        {
            return await this._dbWrapper.Find<EtTenantLcsPayLog>(p => p.OutRefundNo == outTradeNo);
        }

        public async Task EditTenantLcsPayLog(EtTenantLcsPayLog entity)
        {
            await this._dbWrapper.Update(entity);
        }

        public async Task<Tuple<IEnumerable<EtTenantLcsPayLog>, int>> GetTenantLcsPayLogPaging(IPagingRequest request)
        {
            return await this._dbWrapper.ExecutePage<EtTenantLcsPayLog>("EtTenantLcsPayLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task UpdateTenantLcsPayLog(long id, string outTradeNo, string payType, string subAppid, string totalFee)
        {
            await _dbWrapper.Execute($"UPDATE EtTenantLcsPayLog SET OutTradeNo = '{outTradeNo}',PayType = '{payType}',SubAppid = '{subAppid}',TotalFee = '{totalFee}' WHERE Id = {id} AND TenantId = {_tenantId}");
        }
    }
}
