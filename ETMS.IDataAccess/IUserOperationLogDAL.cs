using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IUserOperationLogDAL : IBaseDAL
    {
        Task AddUserLog(EtUserOperationLog userLog);

        Task AddUserLog(RequestBase request, string content, EmUserOperationType type, DateTime? time = null);

        Task<bool> IsUserCanNotBeDelete(long userId);

        Task<bool> IsUserCanNotBeDelete2(long userId);

        Task<Tuple<IEnumerable<UserOperationLogView>, int>> GetPaging(RequestPagingBase request);

        Task<DateTime?> GetLastOpTime(long tenantId);
    }
}
