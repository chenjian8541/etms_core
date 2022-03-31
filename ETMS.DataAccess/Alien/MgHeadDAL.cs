using ETMS.DataAccess.Alien.Core;
using ETMS.DataAccess.Core;
using ETMS.DataAccess.Core.Alien;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.Alien;
using ETMS.Entity.Database.Alien;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.Alien;
using ETMS.Utility;
using System.Threading.Tasks;

namespace ETMS.DataAccess.Alien
{
    public class MgHeadDAL : DataAccessBaseAlien<MgHeadBucket>, IMgHeadDAL
    {
        public MgHeadDAL(IDbWrapperAlien dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<MgHeadBucket> GetDb(params object[] keys)
        {
            var id = keys[0].ToInt();
            var log = await _dbWrapper.Find<MgHead>(p => p.Id == id && p.IsDeleted == EmIsDeleted.Normal);
            return new MgHeadBucket()
            {
                MyMgHead = log
            };
        }

        public async Task AddMgHead(MgHead entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(entity.Id);
        }

        public async Task EditMgHead(MgHead entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(entity.Id);
        }

        public async Task<MgHead> GetMgHead(int id)
        {
            var bucket = await GetCache(id);
            return bucket?.MyMgHead;
        }

        public async Task<MgHead> GetMgHead(string headCode)
        {
            return await _dbWrapper.Find<MgHead>(p => p.IsDeleted == EmIsDeleted.Normal && p.HeadCode == headCode);
        }
    }
}
