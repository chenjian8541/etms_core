using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.Temp.View;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class JobAnalyze2DAL : DataAccessBase, IJobAnalyze2DAL
    {
        public JobAnalyze2DAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<Tuple<IEnumerable<UserPagingView>, int>> GetUserPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<UserPagingView>("EtUser", "Id,Phone", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<StudentPagingView>, int>> GetStudentPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<StudentPagingView>("EtStudent", "Id,Phone,PhoneBak", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<IEnumerable<TempStudentTypeCount>> GetStudentTypeCount()
        {
            var sql = $"SELECT COUNT(StudentType) AS MyCount,StudentType FROM EtStudent WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {_tenantId} GROUP BY StudentType";
            return await _dbWrapper.ExecuteObject<TempStudentTypeCount>(sql);
        }

        public async Task<int> GetUserCount()
        {
            var sql = $"SELECT COUNT(0) FROM EtUser WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {_tenantId} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<int> GetOrderCount()
        {
            var sql = $"SELECT COUNT(0) FROM EtOrder WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {_tenantId} AND OrderType = {EmOrderType.StudentEnrolment} AND [Status] <> {EmOrderStatus.Repeal} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<int> GetClassRecordCount()
        {
            var sql = $"SELECT COUNT(0) FROM EtClassRecord WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {_tenantId} AND [Status] = {EmClassRecordStatus.Normal} ";
            var obj = await _dbWrapper.ExecuteScalar(sql);
            return obj.ToInt();
        }

        public async Task<IEnumerable<TempClassCount>> GetClassCount()
        {
            var sql = $"SELECT COUNT([Type]) AS MyCount,[Type] FROM EtClass WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {_tenantId} AND DataType = {EmClassDataType.Normal} AND CompleteStatus = {EmClassCompleteStatus.UnComplete} GROUP BY [Type]";
            return await _dbWrapper.ExecuteObject<TempClassCount>(sql);
        }
    }
}
