using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.ElectronicAlbum
{
    public interface IElectronicAlbumDAL : IBaseDAL
    {
        Task<EtElectronicAlbum> GetElectronicAlbum(long id);

        Task<EtElectronicAlbum> GetElectronicAlbumByTempId(long tempId);

        Task AddElectronicAlbum(EtElectronicAlbum entity);

        Task EditElectronicAlbum(EtElectronicAlbum entity);

        Task UpdateCIdNo(long id, string cIdNo);

        Task DelElectronicAlbum(long id);

        Task<Tuple<IEnumerable<EtElectronicAlbum>, int>> GetPaging(RequestPagingBase request);

        Task AddReadCount(long id, int addCount);

        Task AddShareCount(long id, int addCount);
    }
}
