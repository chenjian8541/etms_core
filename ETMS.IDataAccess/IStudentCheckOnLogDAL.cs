using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStudentCheckOnLogDAL : IBaseDAL
    {
        Task<EtStudentCheckOnLog> GetStudentCheckOnLog(long id);

        Task<long> AddStudentCheckOnLog(EtStudentCheckOnLog entity);

        Task<bool> EditStudentCheckOnLog(EtStudentCheckOnLog entity);

        Task<Tuple<IEnumerable<EtStudentCheckOnLog>, int>> GetPaging(IPagingRequest request);

        Task<Tuple<IEnumerable<StudentCheckOnLogView>, int>> GetViewPaging(RequestPagingBase request);

        Task<EtStudentCheckOnLog> GetStudentCheckOnLastTime(long studentId);

        Task<List<EtStudentCheckOnLog>> GetStudentCheckOnLogByClassTimesId(long classTimesId);

        Task<EtStudentCheckOnLog> GetStudentDeLog(long classTimesId, long studentId);

        Task<IEnumerable<EtStudentCheckOnLog>> GetStudentDeLog(List<long> classTimesIds, long studentId);

        Task<bool> UpdateStudentCheckOnIsBeRollcall(long classTimesId);

        Task<bool> RevokeCheckSign(long classTimesId);

        Task<int> GetStudentOneDayAttendClassCount(long studentId, DateTime date);

        Task<IEnumerable<OnlyId>> GetOneDayStudentCheckInAllClassTimes(DateTime date);

        Task<DateTime?> GetStudentLastGoClassTime(long studentId);
    }
}
