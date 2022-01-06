using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.ElectronicAlbum
{
    public interface IElectronicAlbumTempDAL: IBaseDAL
    {
        Task AddElectronicAlbumTemp(EtElectronicAlbumTemp entity);

        Task<EtElectronicAlbumTemp> GetElectronicAlbumTemp(long id);
    }
}
