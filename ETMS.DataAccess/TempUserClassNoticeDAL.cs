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

namespace ETMS.DataAccess
{
    public class TempUserClassNoticeDAL : DataAccessBase<TempUserClassNoticeBucket>, ITempUserClassNoticeDAL
    {
        public TempUserClassNoticeDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TempUserClassNoticeBucket> GetDb(params object[] keys)
        {
            var ot = Convert.ToDateTime(keys[1]);
            var data = await _dbWrapper.FindList<EtTempUserClassNotice>(p => p.TenantId == _tenantId && p.Ot == ot && p.IsDeleted == EmIsDeleted.Normal
            && p.Status == EmTempClassNoticeStatus.Normal);
            return new TempUserClassNoticeBucket()
            {
                TempUserClassNotices = data
            };
        }

        public async Task<List<EtTempUserClassNotice>> GetTempUserClassNotice(DateTime classOt)
        {
            var bucket = await GetCache(_tenantId, classOt);
            return bucket?.TempUserClassNotices;
        }

        public async Task GenerateTempUserClassNotice(DateTime classOt)
        {
            var hisOt = classOt.AddDays(-5); //删除5天前的数据
            var delSql = new StringBuilder();
            delSql.Append($"DELETE EtTempUserClassNotice WHERE TenantId = {_tenantId} AND Ot <= '{hisOt.EtmsToDateString()}' ;");
            delSql.Append($"DELETE EtTempUserClassNotice WHERE TenantId = {_tenantId} AND Ot = '{classOt.EtmsToDateString()}' ;");
            await _dbWrapper.Execute(delSql.ToString());
            var classTimes = await _dbWrapper.FindList<EtClassTimes>(p => p.TenantId == _tenantId && p.ClassOt == classOt && p.Status == EmClassTimesStatus.UnRollcall
            && p.IsDeleted == EmIsDeleted.Normal);
            if (classTimes.Count > 0)
            {
                var insertData = new List<EtTempUserClassNotice>();
                foreach (var p in classTimes)
                {
                    var hour = p.StartTime / 100;
                    var minute = p.StartTime % 100;
                    insertData.Add(new EtTempUserClassNotice()
                    {
                        ClassTimesId = p.Id,
                        EndTime = p.EndTime,
                        IsDeleted = EmIsDeleted.Normal,
                        Ot = p.ClassOt,
                        StartTime = p.StartTime,
                        Status = EmTempClassNoticeStatus.Normal,
                        TenantId = p.TenantId,
                        ClassTime = new DateTime(p.ClassOt.Year, p.ClassOt.Month, p.ClassOt.Day, hour, minute, 1)
                    });
                }
                _dbWrapper.InsertRange(insertData);
            }
            await UpdateCache(_tenantId, classOt);
        }

        public async Task SetProcessed(List<long> ids, DateTime classOt)
        {
            string sql;
            if (ids.Count == 1)
            {
                sql = $"UPDATE EtTempUserClassNotice SET [Status] = {EmTempClassNoticeStatus.Processed} WHERE TenantId = {_tenantId} AND Id = {ids[0]} ";
            }
            else
            {
                sql = $"UPDATE EtTempUserClassNotice SET [Status] = {EmTempClassNoticeStatus.Processed} WHERE TenantId = {_tenantId} AND Id IN ({string.Join(',', ids)}) ";
            }
            await _dbWrapper.Execute(sql);
            await UpdateCache(_tenantId, classOt);
        }
    }
}
