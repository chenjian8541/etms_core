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
using System.Linq;
using ETMS.Entity.Temp;
using ETMS.Entity.View.OnlyOneFiled;
using ETMS.Entity.View;

namespace ETMS.DataAccess
{
    public class AchievementDAL : DataAccessBase<AchievementBucket>, IAchievementDAL
    {
        public AchievementDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<AchievementBucket> GetDb(params object[] keys)
        {
            var log = await _dbWrapper.Find<EtAchievement>(p => p.TenantId == _tenantId && p.Id == keys[1].ToLong() && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new AchievementBucket()
            {
                Achievement = log
            };
        }

        public async Task<EtAchievement> GetAchievement(long id)
        {
            var bucket = await GetCache(_tenantId, id);
            return bucket?.Achievement;
        }

        public async Task AddAchievement(EtAchievement entity, List<EtAchievementDetail> details)
        {
            await _dbWrapper.Insert(entity);
            foreach (var p in details)
            {
                p.AchievementId = entity.Id;
            }
            _dbWrapper.InsertRange(details);
        }

        public async Task AddAchievementDetails(List<EtAchievementDetail> entitys)
        {
            await _dbWrapper.InsertRangeAsync(entitys);
        }

        public async Task EditAchievement(EtAchievement entity)
        {
            await _dbWrapper.Update(entity);
            RemoveCache(_tenantId, entity.Id);
        }

        public async Task EditAchievementDetails(List<EtAchievementDetail> details)
        {
            await _dbWrapper.UpdateRange(details);
        }

        public async Task UpdateAchievementDetail(long achievementId, string name, byte showRankParent)
        {
            await _dbWrapper.Execute(
                $"UPDATE EtAchievementDetail SET Name = '{name}',ShowRankParent = {showRankParent} WHERE TenantId = {_tenantId} AND AchievementId = {achievementId}");
        }

        public async Task DelAchievementDetail(List<long> ids)
        {
            if (ids.Count == 0)
            {
                return;
            }
            if (ids.Count == 1)
            {
                await _dbWrapper.Execute($"DELETE EtAchievementDetail WHERE Id = {ids[0]} AND TenantId = {_tenantId}");
            }
            else
            {
                await _dbWrapper.Execute($"DELETE EtAchievementDetail WHERE Id IN ({string.Join(',', ids)}) AND TenantId = {_tenantId}");
            }
        }

        public async Task DelAchievement(long id)
        {
            var strSql = new StringBuilder();
            strSql.Append($"UPDATE EtAchievement SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND Id = {id};");
            strSql.Append($"UPDATE EtAchievementDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND AchievementId = {id};");
            await _dbWrapper.Execute(strSql.ToString());
            RemoveCache(_tenantId, id);
        }

        public async Task PushAchievement(long id)
        {
            var strSql = new StringBuilder();
            strSql.Append($"UPDATE EtAchievement SET [Status] = {EmAchievementStatus.Publish} WHERE TenantId = {_tenantId} AND Id = {id};");
            strSql.Append($"UPDATE EtAchievementDetail SET [Status] = {EmAchievementStatus.Publish} WHERE TenantId = {_tenantId} AND AchievementId = {id};");
            await _dbWrapper.Execute(strSql.ToString());
            RemoveCache(_tenantId, id);
        }

        public async Task<List<EtAchievementDetail>> GetAchievementDetail(long achievementId)
        {
            return await _dbWrapper.FindList<EtAchievementDetail>(p => p.TenantId == _tenantId && p.AchievementId == achievementId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<Tuple<IEnumerable<EtAchievement>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtAchievement>("EtAchievement", "*", request.PageSize, request.PageCurrent, "ExamOt DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtAchievementDetail>, int>> GetPagingDetail(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtAchievementDetail>("EtAchievementDetail", "*", request.PageSize, request.PageCurrent, "ExamOt DESC", request.ToString());
        }
    }
}
