using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class ActiveGrowthRecordDAL : DataAccessBase<ActiveGrowthRecordBucket>, IActiveGrowthRecordDAL
    {
        public ActiveGrowthRecordDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ActiveGrowthRecordBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var activeGrowthRecord = await _dbWrapper.Find<EtActiveGrowthRecord>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Id == id);
            if (activeGrowthRecord == null)
            {
                return null;
            }
            var comments = await _dbWrapper.FindList<EtActiveGrowthRecordDetailComment>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.GrowthRecordId == id);
            return new ActiveGrowthRecordBucket()
            {
                ActiveGrowthRecord = activeGrowthRecord,
                Comments = comments
            };
        }

        public async Task<bool> AddActiveGrowthRecord(EtActiveGrowthRecord entity)
        {
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.Id);
            return true;
        }

        public async Task EditActiveGrowthRecord(EtActiveGrowthRecord entity)
        {
            await this._dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.Id);
        }

        public async Task UpdateActiveGrowthRecordTotalCount(long growthRecordId, int totalCount)
        {
            await _dbWrapper.Execute($"UPDATE [EtActiveGrowthRecord] SET TotalCount = {totalCount} WHERE Id = {growthRecordId} ");
        }

        public async Task<ActiveGrowthRecordBucket> GetActiveGrowthRecord(long id)
        {
            return await GetCache(_tenantId, id);
        }

        public async Task<bool> DelActiveGrowthRecord(long id)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtActiveGrowthRecord SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND Id = {id} ;");
            sql.Append($"UPDATE EtActiveGrowthRecordDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND GrowthRecordId = {id} ;");
            sql.Append($"UPDATE EtActiveGrowthRecordDetailComment SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND GrowthRecordId = {id} ;");
            await _dbWrapper.Execute(sql.ToString());
            RemoveCache(_tenantId, id);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtActiveGrowthRecord>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtActiveGrowthRecord>("EtActiveGrowthRecord", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> AddActiveGrowthRecordDetailComment(EtActiveGrowthRecordDetailComment entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.GrowthRecordId);
            return true;
        }

        public async Task<bool> DelActiveGrowthRecordDetailComment(long growthRecordId, long commentId)
        {
            await _dbWrapper.Execute($"DELETE EtActiveGrowthRecordDetailComment WHERE Id = {commentId} ;");
            await UpdateCache(_tenantId, growthRecordId);
            return true;
        }

        public bool AddActiveGrowthRecordDetail(List<EtActiveGrowthRecordDetail> entitys)
        {
            _dbWrapper.InsertRange(entitys);
            return true;
        }

        public async Task AddActiveGrowthRecordDetail(EtActiveGrowthRecordDetail entity)
        {
            await this._dbWrapper.Insert(entity);
        }

        public async Task<Tuple<IEnumerable<EtActiveGrowthRecordDetail>, int>> GetDetailPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtActiveGrowthRecordDetail>("EtActiveGrowthRecordDetail", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtActiveGrowthRecordDetail> GetActiveGrowthRecordDetail(long growthRecordDetailId)
        {
            return await _dbWrapper.Find<EtActiveGrowthRecordDetail>(growthRecordDetailId);
        }

        public async Task<bool> SetActiveGrowthRecordDetailNewFavoriteStatus(long growthRecordDetailId, byte newFavoriteStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtActiveGrowthRecordDetail SET FavoriteStatus = {newFavoriteStatus} WHERE id = {growthRecordDetailId}");
            return true;
        }

        public async Task<bool> SetActiveGrowthRecordNewFavoriteStatus(long growthRecordId, byte newFavoriteStatus)
        {
            await _dbWrapper.Execute($"UPDATE EtActiveGrowthRecord SET FavoriteStatus = {newFavoriteStatus} WHERE Id = {growthRecordId} AND TenantId = {_tenantId} ");
            return true;
        }

        public async Task<bool> SetActiveGrowthRecordIsRead(long growthRecordId, long growthRecordDetailId)
        {
            var strSql = new StringBuilder();
            strSql.Append($"UPDATE EtActiveGrowthRecord SET ReadStatus = {EmBool.True} WHERE Id = {growthRecordId} AND TenantId = {_tenantId} ;");
            strSql.Append($"UPDATE EtActiveGrowthRecordDetail SET ReadStatus = {EmBool.True} WHERE Id = {growthRecordDetailId} AND TenantId = {_tenantId}; ");
            await _dbWrapper.Execute(strSql.ToString());
            return true;
        }

        public async Task GrowthRecordAddReadCount(long growthRecordId)
        {
            await _dbWrapper.Execute($"UPDATE [EtActiveGrowthRecord] SET ReadCount = ReadCount + 1 WHERE Id = {growthRecordId} ");
        }

        public async Task<IEnumerable<ActiveGrowthRecordDetailSimpleView>> GetActiveGrowthRecordDetailSimpleView(long growthRecordId)
        {
            return await _dbWrapper.ExecuteObject<ActiveGrowthRecordDetailSimpleView>($"SELECT TOP 1000 Id,StudentId,ReadStatus,FavoriteStatus FROM EtActiveGrowthRecordDetail WHERE TenantId = {_tenantId} AND GrowthRecordId = {growthRecordId} AND IsDeleted = {EmIsDeleted.Normal} ");
        }

        public async Task<IEnumerable<GrowthRecordDetailView>> GetGrowthRecordDetailView(long growthRecordId)
        {
            var sql = $"SELECT Id,StudentId FROM EtActiveGrowthRecordDetail WHERE TenantId = {_tenantId} AND GrowthRecordId = {growthRecordId} AND IsDeleted = {EmIsDeleted.Normal} ";
            return await _dbWrapper.ExecuteObject<GrowthRecordDetailView>(sql);
        }

        public async Task DelActiveGrowthRecordDetailAboutRelatedInfo(int sceneType, long relatedId, long studentId)
        {
            var sql = $"UPDATE EtActiveGrowthRecordDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND SceneType = {sceneType} AND RelatedId = {relatedId} ";
            await _dbWrapper.Execute(sql);
        }

        public async Task UpdateActiveGrowthRecordDetail(long growthRecordId, string growthContent, string growthMedias)
        {
            await _dbWrapper.Execute($"UPDATE EtActiveGrowthRecordDetail SET GrowthContent = '{growthContent}',GrowthMedias = '{growthMedias}' WHERE TenantId = {_tenantId} AND GrowthRecordId = {growthRecordId}");
        }
    }
}
