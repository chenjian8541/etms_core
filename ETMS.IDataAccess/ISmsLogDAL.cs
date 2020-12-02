using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ISmsLogDAL : IBaseDAL
    {
        Task AddStudentSmsLog(List<EtStudentSmsLog> logs);

        Task<Tuple<IEnumerable<EtStudentSmsLog>, int>> GetStudentSmsLogPaging(IPagingRequest request);

        Task AddUserSmsLog(List<EtUserSmsLog> logs);

    }
}
