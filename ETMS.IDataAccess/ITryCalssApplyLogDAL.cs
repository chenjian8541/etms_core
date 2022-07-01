using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITryCalssApplyLogDAL : IBaseDAL
    {
        Task<EtTryCalssApplyLog> GetTryCalssApplyLog(long id);

        Task DelTryCalssApplyLog(long id);

        Task<bool> ExistTryCalssApplyLog(long studentId, DateTime classOt);

        Task<bool> AddTryCalssApplyLog(EtTryCalssApplyLog log);

        Task<bool> EditTryCalssApplyLog(EtTryCalssApplyLog log);

        Task<Tuple<IEnumerable<EtTryCalssApplyLog>, int>> GetPaging(IPagingRequest request);

        Task<Tuple<IEnumerable<EtTryCalssApplyLog>, int>> GetPaging2(IPagingRequest request);
    }
}
