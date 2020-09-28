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
    public class ClassRoomDAL : SimpleRepository<EtClassRoom, ClassRoomBucket<EtClassRoom>>, IClassRoomDAL
    {
        public ClassRoomDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        public async Task<bool> AddClassRoom(EtClassRoom entity)
        {
            return await base.AddEntity(entity);
        }

        public async Task DelClassRoom(long id)
        {
            await base.DelEntity(id);
        }

        public async Task<List<EtClassRoom>> GetAllClassRoom()
        {
            return await base.GetAll();
        }
    }
}
