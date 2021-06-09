using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.MicroWeb;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.IDataAccess.MicroWeb;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace ETMS.DataAccess.MicroWeb
{
    public class MicroWebColumnArticleDAL : DataAccessBase<MicroWebColumnArticleBucket>, IMicroWebColumnArticleDAL
    {
        public MicroWebColumnArticleDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }
        protected override async Task<MicroWebColumnArticleBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var log = await _dbWrapper.Find<EtMicroWebColumnArticle>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal && p.TenantId == _tenantId);
            if (log == null)
            {
                return null;
            }
            return new MicroWebColumnArticleBucket()
            {
                MicroWebColumnArticle = log
            };
        }

        public async Task<EtMicroWebColumnArticle> GetMicroWebColumnArticle(long id)
        {
            var bucket = await GetCache(_tenantId, id);
            return bucket?.MicroWebColumnArticle;
        }

        public async Task<bool> AddMicroWebColumnArticle(EtMicroWebColumnArticle entity)
        {
            await this._dbWrapper.Insert(entity);
            return true;
        }

        public async Task<bool> EditMicroWebColumnArticle(EtMicroWebColumnArticle entity)
        {
            await this._dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.Id);
            return true;
        }

        public async Task<bool> DelMicroWebColumnArticle(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtMicroWebColumnArticle SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id} AND TenantId = {_tenantId}");
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<bool> ChangeMicroWebColumnArticleStatus(long id, byte newStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtMicroWebColumnArticle SET [Status] = {newStatus} WHERE Id = {id} AND TenantId = {_tenantId}");
            await UpdateCache(_tenantId, id);
            return true;
        }

        public async Task<bool> AddMicroWebColumnArticleReadCount(long id, int addCount)
        {
            await _dbWrapper.Execute($"UPDATE EtMicroWebColumnArticle SET [ReadCount] = [ReadCount]+{addCount} WHERE Id = {id} AND TenantId = {_tenantId}");
            return true;
        }

        public async Task<Tuple<IEnumerable<EtMicroWebColumnArticle>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtMicroWebColumnArticle>("EtMicroWebColumnArticle", "*", request.PageSize, request.PageCurrent, "[Status] ASC,[UpdateTime] DESC", request.ToString());
        }

        public async Task<EtMicroWebColumnArticle> GetMicroWebColumnSinglePageArticle(long columnId)
        {
            return await this._dbWrapper.Find<EtMicroWebColumnArticle>(p => p.IsDeleted == EmIsDeleted.Normal && p.TenantId == _tenantId && p.ColumnId == columnId);
        }

        public async Task<bool> SaveMicroWebColumnSinglePageArticle(EtMicroWebColumnArticle entity)
        {
            if (entity.Id > 0)
            {
                await this._dbWrapper.Update(entity);
            }
            else
            {
                await this._dbWrapper.Insert(entity);
            }
            return true;
        }

        public async Task<IEnumerable<EtMicroWebColumnArticle>> GetMicroWebColumnArticleTopLimit(long columnId, int limitTop)
        {
            var sql = $"SELECT TOP {limitTop} * FROM EtMicroWebColumnArticle WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {_tenantId} AND ColumnId = {columnId} AND [Status] = {EmMicroWebStatus.Enable} ORDER BY UpdateTime DESC";
            return await this._dbWrapper.ExecuteObject<EtMicroWebColumnArticle>(sql);
        }
    }
}
