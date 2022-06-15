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
    public interface ISysActivityDAL
    {
        Task<SysActivity> GetSysActivity(long id);

        Task<Tuple<IEnumerable<SysActivity>, int>> GetPaging(IPagingRequest request);
    }
}
