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
        Task<bool> TempStudentNeedCheckAdd(List<EtTempStudentNeedCheck> entitys);

        Task<bool> TempStudentNeedCheckSetIsCheckIn(long studentId, DateTime ot);

        Task<bool> TempStudentNeedCheckSetIsCheckOut(long studentId, DateTime ot);

        Task<bool> TempStudentNeedCheckSetIsCheckIn(long id);

        Task<bool> TempStudentNeedCheckSetIsCheckOut(long id);

        Task<Tuple<IEnumerable<EtTempStudentNeedCheck>, int>> TempStudentNeedCheckGetPaging(IPagingRequest request);

        Task<bool> TempStudentNeedCheckClassAdd(List<EtTempStudentNeedCheckClass> entitys);

        Task<bool> TempStudentNeedCheckClassSetIsAttendClass(long classTimesId);

        Task<bool> TempStudentNeedCheckClassSetIsAttendClass(long classTimesId, long studentId);

        Task<bool> TempStudentNeedCheckClassSetIsAttendClassById(long id);

        Task<Tuple<IEnumerable<EtTempStudentNeedCheckClass>, int>> TempStudentNeedCheckClassGetPaging(IPagingRequest request);
    }
}
