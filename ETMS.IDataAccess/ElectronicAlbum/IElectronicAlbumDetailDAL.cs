using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.ElectronicAlbum
{
    public interface IElectronicAlbumDetailDAL : IBaseDAL
    {
        Task<EtElectronicAlbumDetail> GetElectronicAlbumDetail(long id);

        Task AddElectronicAlbumDetail(EtElectronicAlbumDetail entity);

        void AddElectronicAlbumDetail(List<EtElectronicAlbumDetail> entitys);

        Task DelElectronicAlbumDetail(long electronicAlbumId);

        Task EditElectronicAlbumDetail(EtElectronicAlbum myElectronicAlbum);

        Task EditElectronicAlbumDetail(EtElectronicAlbumDetail entity);

        Task<Tuple<IEnumerable<EtElectronicAlbumDetail>, int>> GetPaging(IPagingRequest request);

        Task AddReadCount(long id, int addCount);
    }
}
