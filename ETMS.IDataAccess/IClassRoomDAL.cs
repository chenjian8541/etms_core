using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IClassRoomDAL : IBaseDAL
    {
        Task<bool> AddClassRoom(EtClassRoom entity);

        Task DelClassRoom(long id);

        Task<List<EtClassRoom>> GetAllClassRoom();
    }
}
