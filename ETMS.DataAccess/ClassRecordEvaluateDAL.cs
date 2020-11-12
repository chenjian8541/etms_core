using ETMS.DataAccess.Core;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.DataAccess
{
    public class ClassRecordEvaluateDAL : DataAccessBase, IClassRecordEvaluateDAL
    {
        public ClassRecordEvaluateDAL(IDbWrapper dbWrapper) : base(dbWrapper)
        {
        }

        public async Task<List<EtClassRecordEvaluateStudent>> GetClassRecordEvaluateStudent(long classRecordId, long studentId)
        {
            return await _dbWrapper.FindList<EtClassRecordEvaluateStudent>(p => p.ClassRecordId == classRecordId && p.StudentId == studentId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<Tuple<IEnumerable<EtClassRecordEvaluateStudent>, int>> GetEvaluateStudentPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtClassRecordEvaluateStudent>("EtClassRecordEvaluateStudent", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<Tuple<IEnumerable<EtClassRecordEvaluateTeacher>, int>> GetEvaluateTeacherPaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<EtClassRecordEvaluateTeacher>("EtClassRecordEvaluateTeacher", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }
    }
}
