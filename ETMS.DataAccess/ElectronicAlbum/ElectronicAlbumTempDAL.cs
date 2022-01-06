using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.ElectronicAlbum;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.ICache;
using ETMS.IDataAccess.ElectronicAlbum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess.ElectronicAlbum
{
    public class ElectronicAlbumTempDAL : DataAccessBase<ElectronicAlbumTempBucket>, IElectronicAlbumTempDAL
    {
        public ElectronicAlbumTempDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ElectronicAlbumTempBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var log = await this._dbWrapper.Find<EtElectronicAlbumTemp>(p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new ElectronicAlbumTempBucket()
            {
                ElectronicAlbumTemp = log
            };
        }

        public async Task AddElectronicAlbumTemp(EtElectronicAlbumTemp entity)
        {
            await this._dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.Id);
        }

        public async Task<EtElectronicAlbumTemp> GetElectronicAlbumTemp(long id)
        {
            var bucket = await GetCache(_tenantId, id);
            return bucket?.ElectronicAlbumTemp;
        }
    }
}
