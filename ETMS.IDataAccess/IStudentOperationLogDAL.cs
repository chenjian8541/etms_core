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
    public interface IStudentOperationLogDAL : IBaseDAL
    {
        Task AddStudentLog(EtStudentOperationLog studentLog);

        Task AddStudentLog(long studentId, int tenantId, string content, EmStudentOperationLogType type);

        Task<Tuple<IEnumerable<StudentOperationLogView>, int>> GetPaging(RequestPagingBase request);
    }
}
