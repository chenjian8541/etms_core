using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentSmsLogDAL : IBaseDAL
    {
        Task AddStudentSmsLog(List<EtStudentSmsLog> logs);

        Task<Tuple<IEnumerable<EtStudentSmsLog>, int>> GetOrderPaging(IPagingRequest request);
    }
}
