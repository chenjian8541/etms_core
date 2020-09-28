using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class StudentCourseConsumeLogDAL : DataAccessBase, IStudentCourseConsumeLogDAL
    {
        public StudentCourseConsumeLogDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public void AddStudentCourseConsumeLog(List<EtStudentCourseConsumeLog> studentCourseConsumeLogs)
        {
            _dbWrapper.InsertRange(studentCourseConsumeLogs);
        }

        public async Task AddStudentCourseConsumeLog(EtStudentCourseConsumeLog studentCourseConsumeLogs)
        {
            await _dbWrapper.Insert(studentCourseConsumeLogs);
        }

        public async Task<Tuple<IEnumerable<EtStudentCourseConsumeLog>, int>> GetPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtStudentCourseConsumeLog>("EtStudentCourseConsumeLog", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
