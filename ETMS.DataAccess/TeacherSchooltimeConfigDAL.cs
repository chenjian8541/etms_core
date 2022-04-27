using ETMS.DataAccess.Core;
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
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.Entity.View.Rq;

namespace ETMS.DataAccess
{
    public class TeacherSchooltimeConfigDAL : DataAccessBase<TeacherSchooltimeConfigBucket>, ITeacherSchooltimeConfigDAL
    {

        public TeacherSchooltimeConfigDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TeacherSchooltimeConfigBucket> GetDb(params object[] keys)
        {
            var teahcerId = keys[1].ToLong();
            var teacherSchooltimeConfigsList = await _dbWrapper.FindList<EtTeacherSchooltimeConfig>(
                p => p.TenantId == _tenantId && p.TeacherId == teahcerId && p.IsDeleted == EmIsDeleted.Normal);
            var teacherSchooltimeConfigDetailList = await _dbWrapper.FindList<EtTeacherSchooltimeConfigDetail>(
                p => p.TenantId == _tenantId && p.TeacherId == teahcerId && p.IsDeleted == EmIsDeleted.Normal);

            TeacherSchooltimeConfigExcludeView excludeView = null;
            var teacherSchooltimeConfigExclude = await _dbWrapper.Find<EtTeacherSchooltimeConfigExclude>(
                p => p.TenantId == _tenantId && p.TeacherId == teahcerId && p.IsDeleted == EmIsDeleted.Normal);
            if (teacherSchooltimeConfigExclude != null && !string.IsNullOrEmpty(teacherSchooltimeConfigExclude.ExcludeDateContent))
            {
                excludeView = Newtonsoft.Json.JsonConvert.DeserializeObject<TeacherSchooltimeConfigExcludeView>(teacherSchooltimeConfigExclude.ExcludeDateContent);
            }
            return new TeacherSchooltimeConfigBucket()
            {
                TeacherSchooltimeConfigs = teacherSchooltimeConfigsList,
                EtTeacherSchooltimeConfigDetails = teacherSchooltimeConfigDetailList,
                TeacherSchooltimeConfigExclude = excludeView
            };
        }

        public async Task<TeacherSchooltimeConfigBucket> TeacherSchooltimeConfigGet(long teacherId)
        {
            var bucket=  await GetCache(_tenantId, teacherId);
            if (bucket.TeacherSchooltimeConfigs == null || !bucket.TeacherSchooltimeConfigs.Any())
            {
                return null;
            }
            return bucket;
        }

        public async Task AddTeacherSchooltimeConfig(EtTeacherSchooltimeConfig entity, List<EtTeacherSchooltimeConfigDetail> details)
        {
            await _dbWrapper.Insert(entity);
            foreach (var item in details)
            {
                item.SchooltimeConfigId = entity.Id;
            }
            _dbWrapper.InsertRange(details);
            await UpdateCache(_tenantId, entity.TeacherId);
        }

        public async Task DelTeacherSchooltimeConfig(long schooltimeConfigId, long teacherId)
        {
            var strSql = new StringBuilder();
            strSql.Append($"UPDATE EtTeacherSchooltimeConfig SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND Id = {schooltimeConfigId} ;");
            strSql.Append($"UPDATE EtTeacherSchooltimeConfigDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND SchooltimeConfigId = {schooltimeConfigId} ;");
            await _dbWrapper.Execute(strSql.ToString());
            await UpdateCache(_tenantId, teacherId);
        }

        public async Task SaveTeacherSchooltimeConfigExclude(EtTeacherSchooltimeConfigExclude excludeConfig)
        {
            var hislog = await _dbWrapper.Find<EtTeacherSchooltimeConfigExclude>(
                p => p.TenantId == _tenantId && p.TeacherId == excludeConfig.TeacherId && p.IsDeleted == EmIsDeleted.Normal);
            if (hislog == null)
            {
                await _dbWrapper.Insert(excludeConfig);
            }
            else
            {
                hislog.ExcludeDateContent = excludeConfig.ExcludeDateContent;
                await _dbWrapper.Update(hislog);
            }
            await UpdateCache(_tenantId, excludeConfig.TeacherId);
        }

        public async Task ResetTeacherSchooltimeConfig(ResetTeacherSchooltimeConfigInput input)
        {
            var strSql = new StringBuilder();
            strSql.Append($"UPDATE EtTeacherSchooltimeConfig SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND  TeacherId = {input.TeacherId} ;");
            strSql.Append($"UPDATE EtTeacherSchooltimeConfigDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND  TeacherId = {input.TeacherId} ;");
            strSql.Append($"UPDATE EtTeacherSchooltimeConfigExclude SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND  TeacherId = {input.TeacherId} ;");
            await _dbWrapper.Execute(strSql.ToString());

            var detailList = new List<EtTeacherSchooltimeConfigDetail>();
            foreach (var item in input.Items)
            {
                await _dbWrapper.Insert(item.TeacherSchooltimeConfig);
                foreach (var p in item.TeacherSchooltimeConfigDetails)
                {
                    p.SchooltimeConfigId = item.TeacherSchooltimeConfig.Id;
                    detailList.Add(p);
                }
            }
            await _dbWrapper.InsertRangeAsync(detailList);
            await _dbWrapper.Insert(input.ExcludeConfig);

            await UpdateCache(_tenantId, input.TeacherId);
        }
    }
}
