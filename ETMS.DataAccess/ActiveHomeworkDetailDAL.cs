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
    public class ActiveHomeworkDetailDAL : DataAccessBase<ActiveHomeworkDetailBucket>, IActiveHomeworkDetailDAL
    {
        public ActiveHomeworkDetailDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ActiveHomeworkDetailBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var db = await this._dbWrapper.Find<EtActiveHomeworkDetail>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Id == id);
            if (db == null)
            {
                return null;
            }
            var dbComment = await _dbWrapper.FindList<EtActiveHomeworkDetailComment>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.HomeworkDetailId == id);
            return new ActiveHomeworkDetailBucket()
            {
                ActiveHomeworkDetail = db,
                ActiveHomeworkDetailComments = dbComment
            };
        }

        public bool AddActiveHomeworkDetail(List<EtActiveHomeworkDetail> homeworkDetails)
        {
            _dbWrapper.InsertRange(homeworkDetails);
            return true;
        }

        public async Task<bool> EditActiveHomeworkDetail(EtActiveHomeworkDetail homeworkDetail)
        {
            await this._dbWrapper.Update(homeworkDetail);
            await UpdateCache(_tenantId, homeworkDetail.Id);
            return true;
        }

        public async Task<ActiveHomeworkDetailBucket> GetActiveHomeworkDetailBucket(long id)
        {
            return await GetCache(_tenantId, id);
        }

        public async Task<List<EtActiveHomeworkDetail>> GetActiveHomeworkDetail(long homeworkId, byte answerStatus)
        {
            return await _dbWrapper.FindList<EtActiveHomeworkDetail>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.HomeworkId == homeworkId && p.AnswerStatus == answerStatus);
        }

        public async Task<Tuple<IEnumerable<EtActiveHomeworkDetail>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtActiveHomeworkDetail>("EtActiveHomeworkDetail", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> AddActiveHomeworkDetailComment(EtActiveHomeworkDetailComment detailComment)
        {
            await _dbWrapper.Insert(detailComment);
            await UpdateCache(_tenantId, detailComment.HomeworkDetailId);
            return true;
        }

        public async Task<bool> DelActiveHomeworkDetailComment(long detailId, long id)
        {
            await _dbWrapper.Execute($"DELETE EtActiveHomeworkDetailComment WHERE Id = {id}");
            await UpdateCache(_tenantId, detailId);
            return true;
        }
    }
}
