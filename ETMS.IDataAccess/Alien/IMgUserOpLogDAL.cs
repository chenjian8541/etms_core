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
        Task AddMgUserOpLog(MgUserOpLog entity);

        Task<Tuple<IEnumerable<MgUserOpLog>, int>> GetPaging(IPagingRequest request);
    }
}
