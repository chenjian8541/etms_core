using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.ElectronicAlbum
{
    public interface IElectronicAlbumStatisticsDAL : IBaseDAL
    {
        Task AddReadCount(long electronicAlbumId,DateTime dateTime);

        Task AddShareCount(long electronicAlbumId,DateTime dateTime);

        Task<List<EtElectronicAlbumReadLogDay>> GetReadLog(DateTime startTime, DateTime endTime);

        Task<List<EtElectronicAlbumShareLogDay>> GetShareLog(DateTime startTime, DateTime endTime);
    }
}
