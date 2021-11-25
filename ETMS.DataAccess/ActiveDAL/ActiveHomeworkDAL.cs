using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
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

        public async Task UpdateHomeworkAnswerAndReadCount(long homeworkId, int newReadCount, int newFinishCount)
        {
            await _dbWrapper.Execute($"UPDATE EtActiveHomework SET ReadCount = {newReadCount},FinishCount = {newFinishCount} WHERE Id = {homeworkId} AND TenantId = {_tenantId}");
            await UpdateCache(_tenantId, homeworkId);
        }

        public async Task<List<EtActiveHomework>> GetNeedCreateContinuousHomework(DateTime nowDate)
        {
            return await this._dbWrapper.FindList<EtActiveHomework>(
                p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Type == EmActiveHomeworkType.ContinuousWork
                && p.LxStartDate <= nowDate && p.LxEndDate >= nowDate);
        }

        #region 连续作业
        public void AddActiveHomeworkStudent(List<EtActiveHomeworkStudent> entitys)
        {
            _dbWrapper.InsertRange(entitys);
        }

        public async Task ResetHomeworkStudentAnswerStatus(long homeworkId)
        {
            await _dbWrapper.Execute($"UPDATE EtActiveHomeworkStudent SET AnswerStatus = {EmActiveHomeworkDetailAnswerStatus.Unanswered} WHERE TenantId = {_tenantId} AND HomeworkId = {homeworkId} AND IsDeleted = {EmIsDeleted.Normal} ");
            await _dbWrapper.Execute($"UPDATE EtActiveHomework SET FinishCount = 0 WHERE Id = {homeworkId} AND TenantId = {_tenantId}");
            await UpdateCache(_tenantId, homeworkId);
        }

        public async Task<HomeworkAnswerAndReadCountView> GetAnswerAndReadCount(long homeworkId)
        {
            var result = new HomeworkAnswerAndReadCountView()
            {
                ReadCount = 0,
                AnswerCount = 0
            };
            var readCountObj = await _dbWrapper.ExecuteScalar(
                $"SELECT COUNT(0) AS MyCount FROM EtActiveHomeworkStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND HomeworkId = {homeworkId} AND ReadStatus = {EmActiveHomeworkDetailReadStatus.Yes}");
            if (readCountObj != null)
            {
                result.ReadCount = readCountObj.ToInt();
            }
            var answerCountObj = await _dbWrapper.ExecuteScalar(
                $"SELECT COUNT(0) AS MyCount FROM EtActiveHomeworkStudent WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND HomeworkId = {homeworkId} AND AnswerStatus = {EmActiveHomeworkDetailAnswerStatus.Answered}");
            if (answerCountObj != null)
            {
                result.AnswerCount = answerCountObj.ToInt();
            }
            return result;
        }

        public async Task HomeworkStudentSetReadStatus(long homeworkId, long studentId, byte newStatus)
        {
            await _dbWrapper.Execute(
                $"UPDATE EtActiveHomeworkStudent SET ReadStatus = {newStatus} WHERE TenantId = {_tenantId} AND HomeworkId = {homeworkId} AND StudentId = {studentId} ");
        }

        public async Task HomeworkStudentSetAnswerStatus(long homeworkId, long studentId, byte newStatus)
        {
            await _dbWrapper.Execute(
                $"UPDATE EtActiveHomeworkStudent SET AnswerStatus = {newStatus} WHERE TenantId = {_tenantId} AND HomeworkId = {homeworkId} AND StudentId = {studentId}");
        }

        #endregion 
    }
}
