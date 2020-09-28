using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITryCalssLogDAL : IBaseDAL
    {
        Task<long> AddTryCalssLog(EtTryCalssLog log);

        Task<bool> UpdateStatus(long tryCalssLogId, byte newStatus);

        Task<Tuple<IEnumerable<TryCalssLogView>, int>> GetPaging(RequestPagingBase request);
    }
}
