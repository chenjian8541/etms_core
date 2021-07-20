using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface ITempStudentNeedCheckDAL : IBaseDAL
    {
        Task<TempStudentNeedCheckCountBucket> GetTempStudentNeedCheckCount(DateTime ot);

        Task<EtTempStudentNeedCheck> TempStudentNeedCheckGet(long id);

        Task<bool> TempStudentNeedCheckAdd(List<EtTempStudentNeedCheck> entitys);

        Task<bool> TempStudentNeedCheckSetIsCheckIn(long studentId, DateTime ot);

        Task<bool> TempStudentNeedCheckSetIsCheckOut(long studentId, DateTime ot);

        Task<bool> TempStudentNeedCheckSetIsCheckInById(long id, DateTime ot);

        Task<bool> TempStudentNeedCheckSetIsCheckOutById(long id, DateTime ot);

        Task<Tuple<IEnumerable<EtTempStudentNeedCheck>, int>> TempStudentNeedCheckGetPaging(IPagingRequest request);

        Task<bool> TempStudentNeedCheckClassAdd(List<EtTempStudentNeedCheckClass> entitys);

        Task<bool> TempStudentNeedCheckClassSetIsAttendClass(long classTimesId);

        Task<bool> TempStudentNeedCheckClassSetIsAttendClassByStudentId(long studentId, DateTime date);

        Task<bool> TempStudentNeedCheckClassSetIsAttendClass(long classTimesId, long studentId);

        Task<bool> TempStudentNeedCheckClassSetIsAttendClassById(long id);

        Task<Tuple<IEnumerable<EtTempStudentNeedCheckClass>, int>> TempStudentNeedCheckClassGetPaging(IPagingRequest request);

        Task<EtTempStudentNeedCheckClass> TempStudentNeedCheckClassGet(long id);
    }
}
