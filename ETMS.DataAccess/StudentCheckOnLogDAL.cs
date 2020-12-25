﻿using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
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
using ETMS.Entity.Common;

namespace ETMS.DataAccess
{
    public class StudentCheckOnLogDAL : DataAccessBase<StudentCheckOnLastTimeBucket>, IStudentCheckOnLogDAL
    {
        public StudentCheckOnLogDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StudentCheckOnLastTimeBucket> GetDb(params object[] keys)
        {
            var studentId = keys[1].ToLong();
            var logs = await _dbWrapper.ExecuteObject<EtStudentCheckOnLog>(
                $"SELECT TOP 1 * FROM EtStudentCheckOnLog WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND IsDeleted = {EmIsDeleted.Normal} ORDER BY CheckOt DESC");
            var myLog = logs.FirstOrDefault();
            if (myLog == null)
            {
                return null;
            }
            return new StudentCheckOnLastTimeBucket()
            {
                StudentCheckOnLog = myLog
            };
        }

        public async Task<EtStudentCheckOnLog> GetStudentCheckOnLog(long id)
        {
            return await _dbWrapper.Find<EtStudentCheckOnLog>(p => p.Id == id);
        }

        public async Task<long> AddStudentCheckOnLog(EtStudentCheckOnLog entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.StudentId);
            return entity.Id;
        }

        public async Task<bool> EditStudentCheckOnLog(EtStudentCheckOnLog entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.StudentId);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtStudentCheckOnLog>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStudentCheckOnLog>("EtStudentCheckOnLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtStudentCheckOnLog> GetStudentCheckOnLastTime(long studentId)
        {
            var bucket = await GetCache(_tenantId, studentId);
            return bucket?.StudentCheckOnLog;
        }

        public async Task<List<EtStudentCheckOnLog>> GetStudentCheckOnLogByClassTimesId(long classTimesId)
        {
            return await this._dbWrapper.FindList<EtStudentCheckOnLog>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.ClassTimesId == classTimesId);
        }

        public async Task<EtStudentCheckOnLog> GetStudentDeLog(long classTimesId, long studentId)
        {
            return await this._dbWrapper.Find<EtStudentCheckOnLog>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.ClassTimesId == classTimesId && p.StudentId == studentId && p.Status != EmStudentCheckOnLogStatus.Revoke);
        }

        public async Task<bool> UpdateStudentCheckOnIsBeRollcall(long classTimesId)
        {
            await _dbWrapper.Execute($"update [EtStudentCheckOnLog] set [Status] = {EmStudentCheckOnLogStatus.BeRollcall} where TenantId = {_tenantId} and ClassTimesId = {classTimesId} ");
            return true;
        }
    }
}