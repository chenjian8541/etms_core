using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IIncomeLogDAL : IBaseDAL
    {
        Task<bool> AddIncomeLog(EtIncomeLog etIncomeLog);

        bool AddIncomeLog(List<EtIncomeLog> etIncomeLogs);

        Task<List<EtIncomeLog>> GetIncomeLogByOrderId(long orderId);

        Task<Tuple<IEnumerable<EtIncomeLog>, int>> GetIncomeLogPaging(IPagingRequest request);

        Task<EtIncomeLog> GetIncomeLog(long id);

        Task<bool> EditIncomeLog(EtIncomeLog log);

        Task<bool> DelIncomeLog(List<long> orderIds);

        Task RepealIncomeLog(long projectType, long relationId, long repealUserId, DateTime repealOt);
    }
}
