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
        public StudentAccountRechargeDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StudentAccountRechargeBucket> GetDb(params object[] keys)
        {
            var phone = keys[1].ToString();
            var log = await _dbWrapper.Find<EtStudentAccountRecharge>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Phone == phone);
            if (log == null)
            {
                return null;
            }
            return new StudentAccountRechargeBucket()
            {
                StudentAccountRecharge = log
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

        public async Task<EtStudentAccountRecharge> GetStudentAccountRecharge(string phone)
        {
            var bucket = await GetCache(_tenantId, phone);
            return bucket?.StudentAccountRecharge;
        }

        public async Task<EtStudentAccountRecharge> GetStudentAccountRecharge(long id)
        {
            return await _dbWrapper.Find<EtStudentAccountRecharge>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal && p.Id == id);
        }

        public async Task<Tuple<IEnumerable<EtStudentAccountRecharge>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStudentAccountRecharge>("EtStudentAccountRecharge", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
