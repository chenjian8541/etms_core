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
    public class StudentTagDAL : SimpleRepository<EtStudentTag, StudentTagBucket<EtStudentTag>>, IStudentTagDAL
    {
        public StudentTagDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddStudentTag(EtStudentTag entity)
        {
            return await this.AddEntity(entity);
        }

        public async Task DelStudentTag(long id)
        {
            await this.DelEntity(id);
        }

        public async Task<List<EtStudentTag>> GetAllStudentTag()
        {
            return await this.GetAll();
        }
    }
}
