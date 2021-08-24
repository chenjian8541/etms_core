using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.ICache;
using ETMS.IDataAccess.TeacherSalary;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.TeacherSalary
{
    public class TeacherSalaryPayrollDAL : DataAccessBase<TeacherSalaryPayrollBucket>, ITeacherSalaryPayrollDAL
    {
        public TeacherSalaryPayrollDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TeacherSalaryPayrollBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var teacherSalaryPayroll = await _dbWrapper.Find<EtTeacherSalaryPayroll>(p => p.TenantId == _tenantId
            && p.IsDeleted == EmIsDeleted.Normal && p.Id == id);
            if (teacherSalaryPayroll == null)
            {
                return null;
            }
            var teacherSalaryPayrollUsers = await _dbWrapper.FindList<EtTeacherSalaryPayrollUser>(p => p.TenantId == _tenantId
            && p.IsDeleted == EmIsDeleted.Normal && p.TeacherSalaryPayrollId == id);

            var teacherSalaryPayrollUserDetails = await _dbWrapper.FindList<EtTeacherSalaryPayrollUserDetail>(p => p.TenantId == _tenantId
            && p.IsDeleted == EmIsDeleted.Normal && p.TeacherSalaryPayrollId == id);

            var teacherSalaryPayrollUserPerformances = await _dbWrapper.FindList<EtTeacherSalaryPayrollUserPerformance>(p => p.TenantId == _tenantId
            && p.IsDeleted == EmIsDeleted.Normal && p.TeacherSalaryPayrollId == id);

            return new TeacherSalaryPayrollBucket()
            {
                TeacherSalaryPayroll = teacherSalaryPayroll,
                TeacherSalaryPayrollUsers = teacherSalaryPayrollUsers,
                TeacherSalaryPayrollUserDetails = teacherSalaryPayrollUserDetails,
                TeacherSalaryPayrollUserPerformances = teacherSalaryPayrollUserPerformances
            };
        }

        public async Task<bool> ExistName(string name)
        {
            var obj = await _dbWrapper.Find<EtTeacherSalaryPayroll>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal
            && p.Status != EmTeacherSalaryPayrollStatus.Repeal && p.Name == name);
            return obj != null;
        }

        public async Task<TeacherSalaryPayrollBucket> GetTeacherSalaryPayrollBucket(long id)
        {
            return await GetCache(_tenantId, id);
        }

        public async Task<long> AddTeacherSalaryPayroll(EtTeacherSalaryPayroll entity)
        {
            await this._dbWrapper.Insert(entity);
            return entity.Id;
        }

        public async Task<long> AddTeacherSalaryPayrollUser(EtTeacherSalaryPayrollUser entity)
        {
            await this._dbWrapper.Insert(entity);
            return entity.Id;
        }

        public void AddTeacherSalaryPayrollDetail(List<EtTeacherSalaryPayrollUserDetail> userDetails)
        {
            if (userDetails.Any())
            {
                this._dbWrapper.InsertRange(userDetails);
            }
        }

        public async Task<long> AddTeacherSalaryPayrollUserPerformance(EtTeacherSalaryPayrollUserPerformance entity)
        {
            await this._dbWrapper.Insert(entity);
            return entity.Id;
        }

        public void AddTeacherSalaryPayrollUserPerformanceDetail(List<EtTeacherSalaryPayrollUserPerformanceDetail> entitys)
        {
            if (entitys.Any())
            {
                this._dbWrapper.InsertRange(entitys);
            }
        }

        public async Task<bool> SetTeacherSalaryPayStatus(long teacherSalaryPayrollId, byte newStatus)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtTeacherSalaryPayroll SET [Status] = {newStatus} WHERE Id = {teacherSalaryPayrollId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTeacherSalaryPayrollUser  SET [Status] = {newStatus} WHERE TeacherSalaryPayrollId = {teacherSalaryPayrollId} AND TenantId = {_tenantId} ;");
            await _dbWrapper.Execute(sql.ToString());
            await UpdateCache(_tenantId, teacherSalaryPayrollId);
            return true;
        }

        public async Task<bool> SetTeacherSalaryPayStatusIsOK(long teacherSalaryPayrollId, DateTime payDate)
        {
            var payDateDesc = payDate.EtmsToDateString();
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtTeacherSalaryPayroll SET [Status] = {EmTeacherSalaryPayrollStatus.IsOK},PayDate = '{payDateDesc}' WHERE Id = {teacherSalaryPayrollId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTeacherSalaryPayrollUser  SET [Status] = {EmTeacherSalaryPayrollStatus.IsOK},PayDate = '{payDateDesc}' WHERE TeacherSalaryPayrollId = {teacherSalaryPayrollId} AND TenantId = {_tenantId} ;");
            await _dbWrapper.Execute(sql.ToString());
            await UpdateCache(_tenantId, teacherSalaryPayrollId);
            return true;
        }

        public async Task<bool> UpdatePayValue(long payrollId, TeacherSalaryUpdatePayValue teacherSalaryPayroll, TeacherSalaryUpdatePayValue teacherSalaryPayrollUser,
            List<TeacherSalaryUpdatePayValue> teacherSalaryPayrollUserDetails, List<TeacherSalaryUpdatePayValue> teacherSalaryPayrollUserPerformances)
        {
            var sql = new StringBuilder();
            if (teacherSalaryPayroll != null)
            {
                sql.Append($"UPDATE EtTeacherSalaryPayroll SET PaySum = {teacherSalaryPayroll.NewValue} WHERE Id = {teacherSalaryPayroll.Id} ;");
            }
            if (teacherSalaryPayrollUser != null)
            {
                sql.Append($"UPDATE EtTeacherSalaryPayrollUser SET PayItemSum = {teacherSalaryPayrollUser.NewValue} WHERE Id = {teacherSalaryPayrollUser.Id} ;");
            }
            if (teacherSalaryPayrollUserDetails.Any())
            {
                foreach (var p in teacherSalaryPayrollUserDetails)
                {
                    sql.Append($"UPDATE EtTeacherSalaryPayrollUserDetail SET AmountSum = {p.NewValue} WHERE Id = {p.Id} ;");
                }
            }
            if (teacherSalaryPayrollUserPerformances.Any())
            {
                foreach (var p in teacherSalaryPayrollUserPerformances)
                {
                    sql.Append($"UPDATE EtTeacherSalaryPayrollUserPerformance SET SubmitSum = {p.NewValue} WHERE Id = {p.Id} ;");
                }
            }

            await _dbWrapper.Execute(sql.ToString());
            await UpdateCache(_tenantId, payrollId);
            return true;
        }

        public async Task<bool> DelTeacherSalaryPay(long teacherSalaryPayrollId)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtTeacherSalaryPayroll SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {teacherSalaryPayrollId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTeacherSalaryPayrollUser SET IsDeleted = {EmIsDeleted.Deleted} WHERE TeacherSalaryPayrollId = {teacherSalaryPayrollId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTeacherSalaryPayrollUserDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TeacherSalaryPayrollId = {teacherSalaryPayrollId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTeacherSalaryPayrollUserPerformance SET IsDeleted = {EmIsDeleted.Deleted} WHERE TeacherSalaryPayrollId = {teacherSalaryPayrollId} AND TenantId = {_tenantId} ;");
            sql.Append($"UPDATE EtTeacherSalaryPayrollUserPerformanceDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TeacherSalaryPayrollId = {teacherSalaryPayrollId} AND TenantId = {_tenantId} ;");
            await _dbWrapper.Execute(sql.ToString());

            RemoveCache(_tenantId, teacherSalaryPayrollId);
            return true;
        }

        public async Task<Tuple<IEnumerable<EtTeacherSalaryPayroll>, int>> GetSalaryPayrollPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtTeacherSalaryPayroll>("EtTeacherSalaryPayroll", "*", request.PageSize, request.PageCurrent, "EndDate DESC,Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtTeacherSalaryPayrollUserPerformanceDetail>, int>> GetUserPerformanceDetailPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtTeacherSalaryPayrollUserPerformanceDetail>("EtTeacherSalaryPayrollUserPerformanceDetail", "*", request.PageSize, request.PageCurrent, "ClassOt DESC", request.ToString());
        }

        public async Task<IEnumerable<EtTeacherSalaryPayrollUser>> GetValidSalaryPayrollUser(long userId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);
            var sql = $"SELECT * FROM EtTeacherSalaryPayrollUser WHERE TenantId = {_tenantId} AND UserId = {userId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmTeacherSalaryPayrollStatus.IsOK} AND PayDate >= '{startDate.EtmsToDateString()}' AND PayDate < '{endDate.EtmsToDateString()}'";
            return await _dbWrapper.ExecuteObject<EtTeacherSalaryPayrollUser>(sql);
        }

        public async Task<EtTeacherSalaryPayrollUser> GetTeacherSalaryPayrollUser(long id)
        {
            return await _dbWrapper.Find<EtTeacherSalaryPayrollUser>(p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
        }
    }
}
