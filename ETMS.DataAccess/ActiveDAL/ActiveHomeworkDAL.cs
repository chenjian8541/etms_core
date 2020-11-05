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
    public class ActiveHomeworkDAL : DataAccessBase<ActiveHomeworkBucket>, IActiveHomeworkDAL
    {
        public ActiveHomeworkDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ActiveHomeworkBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var db = await _dbWrapper.Find<EtActiveHomework>(p => p.TenantId == _tenantId && p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
            if (db == null)
            {
                return null;
            }
            return new ActiveHomeworkBucket()
            {
                ActiveHomework = db
            };
        }

        public async Task<bool> AddActiveHomework(EtActiveHomework entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.Id);
            return true;
        }

        public async Task<bool> EditActiveHomework(EtActiveHomework entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.Id);
            return true;
        }

        public async Task<EtActiveHomework> GetActiveHomework(long id)
        {
            var bucket = await GetCache(_tenantId, id);
            return bucket?.ActiveHomework;
        }

        public async Task<bool> DelActiveHomework(long id)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtActiveHomework SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id};");
            sql.Append($"UPDATE EtActiveHomeworkDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE HomeworkId = {id};");
            sql.Append($"UPDATE EtActiveHomeworkDetailComment SET IsDeleted = {EmIsDeleted.Deleted} WHERE HomeworkId = {id};");
            await this._dbWrapper.Execute(sql.ToString());
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtActiveHomework>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtActiveHomework>("EtActiveHomework", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
