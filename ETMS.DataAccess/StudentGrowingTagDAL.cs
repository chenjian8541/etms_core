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
    public class StudentGrowingTagDAL : SimpleRepository<EtStudentGrowingTag, StudentGrowingTagBucket<EtStudentGrowingTag>>, IStudentGrowingTagDAL
    {
        public StudentGrowingTagDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddStudentGrowingTag(EtStudentGrowingTag entity)
        {
            return await this.AddEntity(entity);
        }

        public async Task DelStudentGrowingTag(long id)
        {
            await this.DelEntity(id);
        }

        public async Task<List<EtStudentGrowingTag>> GetAllStudentGrowingTag()
        {
            return await this.GetAll();
        }
    }
}
