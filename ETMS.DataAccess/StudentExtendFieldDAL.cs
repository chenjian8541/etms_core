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
    public class StudentExtendFieldDAL : SimpleRepository<EtStudentExtendField, StudentExtendFieldBucket<EtStudentExtendField>>, IStudentExtendFieldDAL
    {
        public StudentExtendFieldDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddStudentExtendField(EtStudentExtendField entity)
        {
            return await base.AddEntity(entity);
        }

        public async Task DelStudentExtendField(long id)
        {
            await base.DelEntity(id);
        }

        public async Task<List<EtStudentExtendField>> GetAllStudentExtendField()
        {
            return await base.GetAll();
        }
    }
}
