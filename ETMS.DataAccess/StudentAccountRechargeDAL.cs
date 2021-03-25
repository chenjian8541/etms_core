using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentAccountRechargeDAL : DataAccessBase<StudentAccountRechargeBucket>, IStudentAccountRechargeDAL
    {
        private readonly IStudentAccountRechargeBinderCacheDAL _studentAccountRechargeBinderCacheDAL;

        public StudentAccountRechargeDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider,
            IStudentAccountRechargeBinderCacheDAL studentAccountRechargeBinderCacheDAL) : base(dbWrapper, cacheProvider)
        {
            this._studentAccountRechargeBinderCacheDAL = studentAccountRechargeBinderCacheDAL;
        }

        public override void InitTenantId(int tenantId)
        {
            base.InitTenantId(tenantId);
            this._studentAccountRechargeBinderCacheDAL.InitTenantId(tenantId);
        }

        protected override async Task<StudentAccountRechargeBucket> GetDb(params object[] keys)
        {
            var phone = keys[1].ToString();
            var log = await _dbWrapper.Find<EtStudentAccountRecharge>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Phone == phone);
            if (log == null)
            {
                return new StudentAccountRechargeBucket();
            }
            var binders = await _dbWrapper.FindList<EtStudentAccountRechargeBinder>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.StudentAccountRechargeId == log.Id);
            return new StudentAccountRechargeBucket()
            {
                StudentAccountRecharge = log,
                StudentAccountRechargeBinders = binders
            };
        }

        public async Task<bool> ExistStudentAccountRecharge(string phone, long id = 0)
        {
            var log = await _dbWrapper.Find<EtStudentAccountRecharge>(p => p.TenantId == _tenantId
            && p.IsDeleted == EmIsDeleted.Normal && p.Phone == phone && p.Id != id);
            return log != null;
        }

        public async Task<bool> AddStudentAccountRecharge(EtStudentAccountRecharge entity)
        {
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.Phone);
            return true;
        }

        public async Task<bool> EditStudentAccountRecharge(EtStudentAccountRecharge entity)
        {
            await this._dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.Phone);
            return true;
        }

        public async Task<bool> EditStudentAccountRechargePhone(long id, string newPhone, string oldPhone)
        {
            await _dbWrapper.Execute($"UPDATE EtStudentAccountRecharge SET [Phone] = '{newPhone}' WHERE Id = {id} AND TenantId = {_tenantId}");
            RemoveCache(_tenantId, oldPhone);
            await UpdateCache(_tenantId, newPhone);
            return true;
        }

        public async Task<StudentAccountRechargeBucket> GetStudentAccountRecharge(string phone)
        {
            return await GetCache(_tenantId, phone);
        }

        public async Task<EtStudentAccountRecharge> GetStudentAccountRecharge(long id)
        {
            return await _dbWrapper.Find<EtStudentAccountRecharge>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Id == id);
        }

        public async Task<Tuple<IEnumerable<EtStudentAccountRecharge>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStudentAccountRecharge>("EtStudentAccountRecharge", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<EtStudentAccountRechargeBinder> GetAccountRechargeBinderByStudentId(long studentId)
        {
            return await _studentAccountRechargeBinderCacheDAL.GetStudentAccountRechargeBinder(studentId);
        }

        public async Task<bool> StudentAccountRechargeBinderAdd(string phone, EtStudentAccountRechargeBinder entity)
        {
            var binderLog = await GetAccountRechargeBinderByStudentId(entity.StudentId);
            if (binderLog != null)
            {
                return false;
            }
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, phone);
            await _studentAccountRechargeBinderCacheDAL.UpdateStudentAccountRechargeBinder(entity.StudentId);
            return true;
        }

        public async Task<bool> StudentAccountRechargeBinderRemove(string phone, long rechargeBinderId, long studentId)
        {
            await this._dbWrapper.Execute($"UPDATE EtStudentAccountRechargeBinder SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND Id = {rechargeBinderId} ");
            _studentAccountRechargeBinderCacheDAL.RemoveStudentAccountRechargeBinder(studentId);
            await UpdateCache(_tenantId, phone);
            return true;
        }
    }
}
