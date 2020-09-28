using ETMS.DataAccess.Core;
using ETMS.DataAccess.Repository;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Database.Source;
using ETMS.ICache;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class IncomeProjectTypeDAL : SimpleRepository<EtIncomeProjectType, IncomeProjectTypeBucket<EtIncomeProjectType>>, IIncomeProjectTypeDAL
    {
        public IncomeProjectTypeDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddIncomeProjectType(EtIncomeProjectType entity)
        {
            return await base.AddEntity(entity);
        }

        public async Task DelIncomeProjectType(long id)
        {
            await base.DelEntity(id);
        }

        public async Task<List<EtIncomeProjectType>> GetAllIncomeProjectType()
        {
            return await base.GetAll();
        }
    }
}
