using ETMS.DataAccess.Core;
using ETMS.DataAccess.Repository;
using ETMS.Entity.CacheBucket;
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
    public class GradeDAL : SimpleRepository<EtGrade, GradeBucket<EtGrade>>, IGradeDAL
    {
        public GradeDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddGrade(EtGrade entity)
        {
            return await base.AddEntity(entity);
        }

        public async Task DelGrade(long id)
        {
            await base.DelEntity(id);
        }

        public async Task<List<EtGrade>> GetAllGrade()
        {
            return await base.GetAll();
        }

        public async Task<EtGrade> GetGrade(long id)
        {
            return await _dbWrapper.Find<EtGrade>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task EditGrade(EtGrade entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_tenantId);
        }
    }
}
