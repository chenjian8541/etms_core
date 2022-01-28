using ETMS.DataAccess.Core;
using ETMS.DataAccess.Lib;
using ETMS.Entity.CacheBucket.EtmsManage;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.View;
using ETMS.ICache;
using ETMS.IDataAccess.EtmsManage;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.EtmsManage
{
    public class SysElectronicAlbumDAL : BaseCacheDAL<SysElectronicAlbumBucket>, ISysElectronicAlbumDAL, IEtmsManage
    {
        public SysElectronicAlbumDAL(ICacheProvider cacheProvider) : base(cacheProvider)
        {
        }

        protected override async Task<SysElectronicAlbumBucket> GetDb(params object[] keys)
        {
            var log = await this.Find<SysElectronicAlbum>(p => p.Id == keys[0].ToLong());
            if (log == null)
            {
                return null;
            }
            return new SysElectronicAlbumBucket()
            {
                ElectronicAlbum = log
            };
        }

        public async Task<SysElectronicAlbum> GetElectronicAlbum(long id)
        {
            var bucket = await GetCache(id);
            return bucket?.ElectronicAlbum;
        }

        public async Task AddElectronicAlbum(SysElectronicAlbum entity)
        {
            await this.Insert(entity);
            await UpdateCache(entity.Id);
        }

        public async Task EditElectronicAlbum(SysElectronicAlbum entity)
        {
            await this.Update(entity);
            await UpdateCache(entity.Id);
        }

        public async Task<Tuple<IEnumerable<SysElectronicAlbum>, int>> GetPaging(IPagingRequest request)
        {
            return await this.ExecutePage<SysElectronicAlbum>("SysElectronicAlbum", "*", request.PageSize, request.PageCurrent, "OrderIndex ASC", request.ToString());
        }
    }
}
