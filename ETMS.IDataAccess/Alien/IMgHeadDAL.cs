using ETMS.Entity.Common;
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
        Task<bool> ExistHeadCode(string headCode, int id = 0);

        Task AddMgHead(MgHead entity);

        Task EditMgHead(MgHead entity);

        Task<MgHead> GetMgHead(int id);

        Task<MgHead> GetMgHead(string headCode);

        Task DelMgHead(int id);

        Task<Tuple<IEnumerable<MgHead>, int>> GetPaging(IPagingRequest request);

        Task UpdateTenantCount(int id);
    }
}
