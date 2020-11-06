using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class ActiveWxMessageDAL : DataAccessBase<ActiveWxMessageBucket>, IActiveWxMessageDAL
    {
        public ActiveWxMessageDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ActiveWxMessageBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var db = await _dbWrapper.Find<EtActiveWxMessage>(p => p.TenantId == _tenantId && p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
            if (db == null)
            {
                return null;
            }
            return new ActiveWxMessageBucket()
            {
                ActiveWxMessage = db
            };
        }

        public async Task<bool> AddActiveWxMessage(EtActiveWxMessage entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.Id);
            return true;
        }

        public async Task<bool> AddActiveWxMessageCount(long id, int addCount)
        {
            await this._dbWrapper.Execute($"UPDATE EtActiveWxMessage SET TotalCount = TotalCount + {addCount} WHERE id = {id} ");
            await UpdateCache(_tenantId, id);
            return true;
        }

        public async Task<EtActiveWxMessage> GetActiveWxMessage(long id)
        {
            var bucket = await GetCache(_tenantId, id);
            return bucket?.ActiveWxMessage;
        }

        public async Task<bool> EditActiveWxMessage(EtActiveWxMessage entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.Id);
            return true;
        }

        public async Task<bool> DelActiveWxMessage(long id)
        {
            var sql = new StringBuilder();
            sql.Append($"DELETE EtActiveWxMessage WHERE Id = {id} AND TenantId = {_tenantId} ;");
            sql.Append($"DELETE EtActiveWxMessageDetail WHERE WxMessageId = {id} AND TenantId = {_tenantId} ;");
            await this._dbWrapper.Execute(sql.ToString());
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtActiveWxMessage>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtActiveWxMessage>("EtActiveWxMessage", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public bool AddActiveWxMessageDetail(List<EtActiveWxMessageDetail> entitys)
        {
            this._dbWrapper.InsertRange(entitys);
            return true;
        }

        public async Task<bool> EditActiveWxMessageDetail(EtActiveWxMessageDetail entity)
        {
            await this._dbWrapper.Update(entity);
            return true;
        }

        public async Task<EtActiveWxMessageDetail> GetActiveWxMessageDetail(long id)
        {
            return await this._dbWrapper.Find<EtActiveWxMessageDetail>(id);
        }

        public async Task<Tuple<IEnumerable<EtActiveWxMessageDetail>, int>> GetDetailPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtActiveWxMessageDetail>("EtActiveWxMessageDetail", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> SyncWxMessageDetail(long wxMessageId, string title, byte IsNeedConfirm)
        {
            await this._dbWrapper.Execute($"UPDATE EtActiveWxMessageDetail SET Title = '{title}',IsNeedConfirm = {IsNeedConfirm} WHERE WxMessageId = {wxMessageId} AND TenantId = {_tenantId} ");
            return true;
        }
    }
}
