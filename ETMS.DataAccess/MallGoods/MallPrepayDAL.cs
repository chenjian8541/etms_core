using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.CacheBucket.Mall;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Parent3.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess.MallGoodsDAL;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.MallGoods
{
    public class MallPrepayDAL : DataAccessBase<MallPrepayBucket>, IMallPrepayDAL
    {
        public MallPrepayDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<MallPrepayBucket> GetDb(params object[] keys)
        {
            var lcsPayLogId = keys[1].ToLong();
            var log = await _dbWrapper.Find<EtMallPrepay>(p => p.TenantId == _tenantId
            && p.LcsPayLogId == lcsPayLogId && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            var req = JsonConvert.DeserializeObject<ParentBuyMallGoodsSubmitRequest>(log.ReqContent);
            return new MallPrepayBucket()
            {
                MallCartView = new MallPrepayView()
                {
                    Id = log.Id,
                    LcsPayLogId = log.LcsPayLogId,
                    Status = log.Status,
                    Type = log.Type,
                    Request = req
                }
            };
        }

        private async Task DelHistoryLog()
        {
            var hisDate = DateTime.Now.AddDays(-7);
            await _dbWrapper.Execute($"DELETE EtMallPrepay WHERE CreateTime <= '{hisDate.EtmsToDateString()}' AND TenantId = {_tenantId}");
        }

        public async Task MallPrepayAdd(EtMallPrepay entity)
        {
            await DelHistoryLog();
            await _dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.LcsPayLogId);
        }

        public async Task<MallPrepayBucket> MallPrepayGetBucket(long lcsPayLogId)
        {
            return await GetCache(_tenantId, lcsPayLogId);
        }

        public async Task<EtMallPrepay> MallPrepayGet(long lcsPayLogId)
        {
            return await _dbWrapper.Find<EtMallPrepay>(p => p.LcsPayLogId == lcsPayLogId && p.TenantId == _tenantId
            && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task UpdateMallPrepayStatus(long lcsPayLogId, byte newStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtMallPrepay SET [Status] = {newStatus} WHERE TenantId = {_tenantId} AND LcsPayLogId = {lcsPayLogId} AND IsDeleted = {EmIsDeleted.Normal} ");
            RemoveCache(_tenantId, lcsPayLogId);
        }
    }
}
