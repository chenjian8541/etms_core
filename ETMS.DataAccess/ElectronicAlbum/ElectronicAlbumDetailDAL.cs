using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket.ElectronicAlbum;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
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
    public class ElectronicAlbumDetailDAL : DataAccessBase, IElectronicAlbumDetailDAL
    {
        public ElectronicAlbumDetailDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task<EtElectronicAlbumDetail> GetElectronicAlbumDetail(long id)
        {
            return await _dbWrapper.Find<EtElectronicAlbumDetail>(
                p => p.Id == id && p.TenantId == _tenantId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task AddElectronicAlbumDetail(EtElectronicAlbumDetail entity)
        {
            await this._dbWrapper.Insert(entity);
        }

        public void AddElectronicAlbumDetail(List<EtElectronicAlbumDetail> entitys)
        {
            this._dbWrapper.InsertRange(entitys);
        }

        public async Task DelElectronicAlbumDetail(long electronicAlbumId)
        {
            await _dbWrapper.Execute($"UPDATE EtElectronicAlbumDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE ElectronicAlbumId = {electronicAlbumId}");
        }

        public async Task EditElectronicAlbumDetail(EtElectronicAlbum myElectronicAlbum)
        {
            await _dbWrapper.Execute(
                $"UPDATE EtElectronicAlbumDetail SET Name = '{myElectronicAlbum.Name}',CoverKey = '{myElectronicAlbum.CoverKey}',RenderKey = '{myElectronicAlbum.RenderKey}',[Status] = {myElectronicAlbum.Status} WHERE ElectronicAlbumId = {myElectronicAlbum.Id} AND TenantId = {_tenantId}");
        }

        public async Task EditElectronicAlbumDetail(EtElectronicAlbumDetail entity)
        {
            await _dbWrapper.Update(entity);
        }

        public async Task<Tuple<IEnumerable<EtElectronicAlbumDetail>, int>> GetPaging(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtElectronicAlbumDetail>("EtElectronicAlbumDetail", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task AddReadCount(long id, int addCount)
        {
            await _dbWrapper.Execute($"UPDATE EtElectronicAlbumDetail SET ReadCount = ReadCount + {addCount} WHERE Id = {id}");
        }

        public async Task AddShareCount(long id, int addCount)
        {
            await _dbWrapper.Execute($"UPDATE EtElectronicAlbumDetail SET ShareCount = ShareCount + {addCount} WHERE Id = {id}");
        }

        public async Task<IEnumerable<ElectronicAlbumDetailSimpleView>> GetElectronicAlbumDetailSimple(long electronicAlbumId)
        {
            return await _dbWrapper.ExecuteObject<ElectronicAlbumDetailSimpleView>(
                $"SELECT Id,StudentId FROM EtElectronicAlbumDetail WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND ElectronicAlbumId = {electronicAlbumId}");
        }
    }
}
