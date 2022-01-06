using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.ElectronicAlbum;
using ETMS.Entity.Common;
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
    public class ElectronicAlbumDAL : DataAccessBase<ElectronicAlbumBucket>, IElectronicAlbumDAL
    {
        public ElectronicAlbumDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<ElectronicAlbumBucket> GetDb(params object[] keys)
        {
            var id = keys[1].ToLong();
            var log = await this._dbWrapper.Find<EtElectronicAlbum>(p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
            if (log == null)
            {
                return null;
            }
            return new ElectronicAlbumBucket()
            {
                ElectronicAlbum = log
            };
        }

        public async Task<EtElectronicAlbum> GetElectronicAlbum(long id)
        {
            var bucket = await GetCache(_tenantId, id);
            return bucket?.ElectronicAlbum;
        }

        public async Task<EtElectronicAlbum> GetElectronicAlbumByTempId(long tempId)
        {
            return await _dbWrapper.Find<EtElectronicAlbum>(p => p.TenantId == _tenantId && p.TempId == tempId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task AddElectronicAlbum(EtElectronicAlbum entity)
        {
            await _dbWrapper.Insert(entity);
            await UpdateCache(_tenantId, entity.Id);
        }

        public async Task EditElectronicAlbum(EtElectronicAlbum entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.Id);
        }

        public async Task UpdateCIdNo(long id, string cIdNo)
        {
            await _dbWrapper.Execute($"UPDATE EtElectronicAlbum SET CIdNo = '{cIdNo}' WHERE Id = {id}");
            await UpdateCache(_tenantId, id);
        }

        public async Task DelElectronicAlbum(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtElectronicAlbum SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id}");
            RemoveCache(_tenantId, id);
        }

        public async Task<Tuple<IEnumerable<EtElectronicAlbum>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtElectronicAlbum>("EtElectronicAlbum", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}