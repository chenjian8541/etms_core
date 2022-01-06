using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.EtmsManage.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysElectronicAlbumDAL
    {
        Task<SysElectronicAlbum> GetElectronicAlbum(long id);

        Task AddElectronicAlbum(SysElectronicAlbum entity);

        Task EditElectronicAlbum(SysElectronicAlbum entity);

        Task<Tuple<IEnumerable<SysElectronicAlbum>, int>> GetPaging(IPagingRequest request);
    }
}
