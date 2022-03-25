using ETMS.Entity.Database.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Alien
{
    public interface IMgHeadDAL
    {
        Task AddMgHead(MgHead entity);

        Task EditMgHead(MgHead entity);

        Task<MgHead> GetMgHead(int id);

        Task<MgHead> GetMgHead(string headCode);
    }
}
