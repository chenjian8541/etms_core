using ETMS.Entity.Alien.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Alien
{
    public interface IMgUserOpLogDAL: IBaseAlienDAL
    {
        Task AddUserOpLog(MgUserOpLog entity);

        Task AddUserLog(AlienRequestBase request,string content,int opType);

        Task<Tuple<IEnumerable<MgUserOpLog>, int>> GetPaging(IPagingRequest request);
    }
}
