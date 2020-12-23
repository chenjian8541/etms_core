using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class TempStudentNeedCheckDAL : DataAccessBase, ITempStudentNeedCheckDAL
    {
        public TempStudentNeedCheckDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<bool> TempStudentNeedCheckAdd(List<EtTempStudentNeedCheck> entitys)
        {
            await TempStudentNeedCheckDelOldHis();
            foreach (var entity in entitys)
            {
                var hisLog = await _dbWrapper.Find<EtTempStudentNeedCheck>(p => p.TenantId == _tenantId && p.StudentId == entity.StudentId && p.Ot == entity.Ot && p.IsDeleted == EmIsDeleted.Normal);
                if (hisLog == null)
                {
                    await _dbWrapper.Insert(entity);
                }
                else
                {
                    hisLog.ClassDesc = $"{hisLog.ClassDesc},{entity.ClassDesc}";
                    await _dbWrapper.Update(entity);
                }
            }
            return true;
        }

        private async Task<bool> TempStudentNeedCheckDelOldHis()
        {
            var hisOt = DateTime.Now.AddDays(-5); //删除5天前的数据
            await _dbWrapper.Execute($"DELETE EtTempStudentNeedCheck WHERE TenantId = {_tenantId} AND Ot <= '{hisOt.EtmsToDateString()}' ");
            return true;
        }

        public async Task<bool> TempStudentNeedCheckSetIsCheckIn(long studentId, DateTime ot)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheck SET IsCheckIn = {EmBool.True} WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND Ot = '{ot.EtmsToDateString()}'");
            return true;
        }

        public async Task<bool> TempStudentNeedCheckSetIsCheckOut(long studentId, DateTime ot)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheck SET IsCheckOut = {EmBool.True} WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND Ot = '{ot.EtmsToDateString()}'");
            return true;
        }

        public async Task<bool> TempStudentNeedCheckSetIsCheckIn(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheck SET IsCheckIn = {EmBool.True} WHERE TenantId = {_tenantId} AND Id = {id} ");
            return true;
        }

        public async Task<bool> TempStudentNeedCheckSetIsCheckOut(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheck SET IsCheckOut = {EmBool.True} WHERE TenantId = {_tenantId} AND Id = {id} ");
            return true;
        }

        public async Task<Tuple<IEnumerable<EtTempStudentNeedCheck>, int>> TempStudentNeedCheckGetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtTempStudentNeedCheck>("EtTempStudentNeedCheck", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<bool> TempStudentNeedCheckClassAdd(List<EtTempStudentNeedCheckClass> entitys)
        {
            await TempStudentNeedCheckClassDelOldHis();
            if (entitys != null && entitys.Count > 0)
            {
                _dbWrapper.InsertRange(entitys);
            }
            return true;
        }

        private async Task<bool> TempStudentNeedCheckClassDelOldHis()
        {
            var hisOt = DateTime.Now.AddDays(-5); //删除5天前的数据
            await _dbWrapper.Execute($"DELETE EtTempStudentNeedCheckClass WHERE TenantId = {_tenantId} AND Ot <= '{hisOt.EtmsToDateString()}' ");
            return true;
        }

        public async Task<bool> TempStudentNeedCheckClassSetIsAttendClass(long classTimesId)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheckClass SET [Status] = {EmTempStudentNeedCheckClassStatus.IsAttendClass} WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimesId} ");
            return true;
        }

        public async Task<bool> TempStudentNeedCheckClassSetIsAttendClass(long classTimesId, long studentId)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheckClass SET [Status] = {EmTempStudentNeedCheckClassStatus.IsAttendClass} WHERE TenantId = {_tenantId} AND ClassTimesId = {classTimesId} AND StudentId = {studentId} ");
            return true;
        }

        public async Task<bool> TempStudentNeedCheckClassSetIsAttendClassById(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtTempStudentNeedCheckClass SET [Status] = {EmTempStudentNeedCheckClassStatus.IsAttendClass} WHERE TenantId = {_tenantId} AND Id = {id} ");
            return true;
        }

        public async Task<Tuple<IEnumerable<EtTempStudentNeedCheckClass>, int>> TempStudentNeedCheckClassGetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtTempStudentNeedCheckClass>("EtTempStudentNeedCheckClass", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
