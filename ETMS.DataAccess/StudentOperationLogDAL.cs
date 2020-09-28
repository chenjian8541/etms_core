using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentOperationLogDAL : DataAccessBase, IStudentOperationLogDAL
    {
        public StudentOperationLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        { }

        public async Task AddStudentLog(EtStudentOperationLog studentLog)
        {
            await _dbWrapper.Insert(studentLog);
        }

        public async Task AddStudentLog(long studentId, int tenantId, string content, EmStudentOperationLogType type)
        {
            await AddStudentLog(new EtStudentOperationLog()
            {
                IpAddress = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                OpContent = content,
                Ot = DateTime.Now,
                TenantId = tenantId,
                StudentId = studentId,
                Type = (int)type
            });
        }

        public async Task<Tuple<IEnumerable<StudentOperationLogView>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<StudentOperationLogView>("StudentOperationLogView", "*", request.PageSize, request.PageCurrent, "Ot DESC", request.ToString());
        }
    }
}
