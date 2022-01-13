using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp.View;
using ETMS.Entity.View;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Utility;
using ETMS.Entity.View.OnlyOneFiled;
using ETMS.Entity.View.LibMedia;

namespace ETMS.DataAccess
{
    public class LibMediaDAL : DataAccessBase, ILibMediaDAL
    {
        public LibMediaDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<int> GetImageCount(int type)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT COUNT(0) FROM EtLibImages WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Type] = {type}");
            return obj.ToInt();
        }

        public async Task<int> GetAudioCount(int type)
        {
            var obj = await _dbWrapper.ExecuteScalar(
                $"SELECT COUNT(0) FROM EtLibAudios WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Type] = {type}");
            return obj.ToInt();
        }

        public async Task AddImage(EtLibImages entity)
        {
            await this._dbWrapper.Insert(entity);
        }

        public async Task DelImage(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtLibImages SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id}");
        }

        public async Task AddAudio(EtLibAudios entity)
        {
            await this._dbWrapper.Insert(entity);
        }

        public async Task DelAudio(long id)
        {
            await _dbWrapper.Execute($"UPDATE EtLibAudios SET IsDeleted = {EmIsDeleted.Deleted} WHERE Id = {id}");
        }

        public async Task<Tuple<IEnumerable<EtLibImages>, int>> GetPagingImg(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtLibImages>("EtLibImages", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtLibAudios>, int>> GetPagingAudio(IPagingRequest request)
        {
            return await _dbWrapper.ExecutePage<EtLibAudios>("EtLibAudios", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<IEnumerable<LibImageView>> GetImages(int type)
        {
            return await _dbWrapper.ExecuteObject<LibImageView>(
                  $"SELECT TOP 200 Id,ImgUrl FROM EtLibImages WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {_tenantId} AND [Type] = {type}  ORDER BY Id DESC");
        }

        public async Task<IEnumerable<LibAudioView>> GetAudios(int type)
        {
            return await _dbWrapper.ExecuteObject<LibAudioView>(
                $"SELECT TOP 200 Id,AudioUrl FROM LibAudioView WHERE IsDeleted = {EmIsDeleted.Normal} AND TenantId = {_tenantId} AND [Type] = {type}  ORDER BY Id DESC");
        }
    }
}
