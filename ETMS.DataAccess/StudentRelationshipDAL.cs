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
    public class StudentRelationshipDAL : SimpleRepository<EtStudentRelationship, StudentRelationshipBucket<EtStudentRelationship>>, IStudentRelationshipDAL
    {
        public StudentRelationshipDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddStudentRelationship(EtStudentRelationship entity)
        {
            return await this.AddEntity(entity);
        }

        public async Task DelStudentRelationship(long id)
        {
            await this.DelEntity(id);
        }

        public async Task<List<EtStudentRelationship>> GetAllStudentRelationship()
        {
            return await this.GetAll();
        }
    }
}
