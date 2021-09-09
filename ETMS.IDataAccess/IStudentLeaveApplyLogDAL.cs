using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentLeaveApplyLogDAL : IBaseDAL
    {
        Task<Tuple<IEnumerable<StudentLeaveApplyLogView>, int>> GetPaging(IPagingRequest request);

        Task<bool> AddStudentLeaveApplyLog(EtStudentLeaveApplyLog log);

        Task<bool> EditStudentLeaveApplyLog(EtStudentLeaveApplyLog log);

        Task<EtStudentLeaveApplyLog> GetStudentLeaveApplyLog(long id);

        Task<List<EtStudentLeaveApplyLog>> GetStudentLeaveApplyPassLog(DateTime time);

        Task<int> GetStudentLeaveApplyCount(long studentId,DateTime startTime, DateTime endTime);

        Task<bool> ExistStudentLeaveApplyLog(long studentId, DateTime startFullTime, DateTime endFullTime);
    }
}
