using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.TeacherSalary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.TeacherSalary
{
    public class TeacherSalaryFundsItemsDAL : DataAccessBase<TeacherSalaryFundsItemsBucket>, ITeacherSalaryFundsItemsDAL
    {
        public TeacherSalaryFundsItemsDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<TeacherSalaryFundsItemsBucket> GetDb(params object[] keys)
        {
            var logs = await _dbWrapper.FindList<EtTeacherSalaryFundsItems>(p => p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            return new TeacherSalaryFundsItemsBucket()
            {
                TeacherSalaryFundsItemsList = logs.OrderBy(p => p.Id).ToList()
            };
        }

        public async Task<bool> AddTeacherSalaryFundsItems(EtTeacherSalaryFundsItems entity)
        {
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_tenantId);
            return true;
        }

        public async Task<List<EtTeacherSalaryFundsItems>> GetTeacherSalaryFundsItems()
        {
            var bucket = await GetCache(_tenantId);
            return bucket?.TeacherSalaryFundsItemsList;
        }

        public async Task<bool> DelTeacherSalaryFundsItems(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtTeacherSalaryFundsItems SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id} ");
            await UpdateCache(_tenantId);
            return true;
        }
    }
}
