using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentTrackLogDAL : IBaseDAL
    {
        Task<long> AddStudentTrackLog(EtStudentTrackLog etStudentTrackLog);

        Task<bool> AddStudentTrackLog(List<EtStudentTrackLog> etStudentTrackLogs);

        Task<List<EtStudentTrackLog>> GetStudentTrackLog(long studentId);

        Task<bool> EditStudentTrackLog(EtStudentTrackLog etStudentTrackLog);

        Task<EtStudentTrackLog> GetTrackLog(long id);

        Task<bool> DelStudentTrackLog(long id, long studentId);

        Task<Tuple<IEnumerable<StudentTrackLogView>, int>> GetPaging(RequestPagingBase request);
    }
}
