using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.Entity.View.OnlyOneFiled;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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

        public async Task<List<EtActiveHomeworkDetail>> GetActiveHomeworkDetail(long homeworkId, byte answerStatus, DateTime? otDate, long? studentId)
        {
            if (otDate != null)
            {
                return await _dbWrapper.FindList<EtActiveHomeworkDetail>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
                && p.HomeworkId == homeworkId && p.AnswerStatus == answerStatus && p.OtDate == otDate.Value);
            }
            if (studentId != null)
            {
                return await _dbWrapper.FindList<EtActiveHomeworkDetail>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
                && p.HomeworkId == homeworkId && p.AnswerStatus == answerStatus && p.StudentId == studentId.Value);
            }
            return await _dbWrapper.FindList<EtActiveHomeworkDetail>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.HomeworkId == homeworkId && p.AnswerStatus == answerStatus);
        }

        public async Task<List<EtActiveHomeworkDetail>> GetActiveHomeworkDetail(long homeworkId, DateTime otDate)
        {
            return await _dbWrapper.FindList<EtActiveHomeworkDetail>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.HomeworkId == homeworkId && p.OtDate == otDate);
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

        public async Task<IEnumerable<EtActiveHomeworkDetail>> GetHomeworkDetailTomorrowSingleWorkExDate()
        {
            var tomorrow = DateTime.Now.AddDays(1);
            var afterTomorrow = DateTime.Now.AddDays(2);
            return await _dbWrapper.ExecuteObject<EtActiveHomeworkDetail>(
                $"SELECT TOP 500 * FROM EtActiveHomeworkDetail WHERE TenantId = {_tenantId} AND [Type] = {EmActiveHomeworkType.SingleWork}  AND IsDeleted = {EmIsDeleted.Normal} AND AnswerStatus = {EmActiveHomeworkDetailAnswerStatus.Unanswered} AND ExDate < '{afterTomorrow.EtmsToDateString()}' AND ExDate >= '{tomorrow.EtmsToDateString()}'");
        }

        public async Task<IEnumerable<EtActiveHomeworkDetail>> GetHomeworkDetailContinuousWorkTodayNotAnswer(DateTime otDate, int maxTime, int minTime)
        {
            return await _dbWrapper.ExecuteObject<EtActiveHomeworkDetail>(
                $"SELECT TOP 500 * FROM EtActiveHomeworkDetail WHERE TenantId = {_tenantId} AND [Type] = {EmActiveHomeworkType.ContinuousWork} AND IsDeleted = {EmIsDeleted.Normal} AND AnswerStatus = {EmActiveHomeworkDetailAnswerStatus.Unanswered} AND OtDate = '{otDate.EtmsToDateString()}' AND LxExTime >= {minTime} AND LxExTime <= {maxTime}");
        }

        public async Task<byte> GetHomeworkStudentAnswerStatus(long homeworkId, long studentId)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtActiveHomeworkDetail WHERE TenantId = {_tenantId} AND HomeworkId = {homeworkId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal} AND AnswerStatus = {EmActiveHomeworkDetailAnswerStatus.Unanswered}");
            if (obj != null)
            {
                return EmActiveHomeworkDetailAnswerStatus.Unanswered;
            }
            return EmActiveHomeworkDetailAnswerStatus.Answered;
        }

        public async Task<bool> ExistHomeworkDetail(long homeworkId, DateTime otDate)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT TOP 1 0 FROM EtActiveHomeworkDetail WHERE TenantId = {_tenantId} AND HomeworkId = {homeworkId} AND OtDate = '{otDate.EtmsToDateString()}' AND IsDeleted = {EmIsDeleted.Normal}");
            return obj != null;
        }

        public async Task UpdateHomeworkDetail(long homeworkId, string title, string workContent, string workMedias)
        {
            var myOnlyOneFileds = await _dbWrapper.ExecuteObject<OnlyOneFiledId>($"SELECT top 200 Id FROM EtActiveHomeworkDetail WHERE HomeworkId = {homeworkId}");
            if (myOnlyOneFileds.Any())
            {
                foreach (var item in myOnlyOneFileds)
                {
                    RemoveCache(_tenantId, item.Id);
                }
            }
            await _dbWrapper.Execute($"UPDATE EtActiveHomeworkDetail SET Title = '{title}',WorkContent = '{workContent}',WorkMedias = '{workMedias}' WHERE HomeworkId = {homeworkId}");
        }
    }
}
