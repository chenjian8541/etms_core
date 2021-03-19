using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IClassTimesDAL : IBaseDAL
    {
        Task<long> AddClassTimes(EtClassTimes classTime);

        bool AddClassTimes(IEnumerable<EtClassTimes> classTimes);

        Task<EtClassTimes> GetClassTimes(long id);

        Task<bool> EditClassTimes(EtClassTimes classTime);

        Task<bool> DelClassTimes(long id);

        Task<List<EtClassTimesStudent>> GetClassTimesStudent(long classTimesId);

        Task<EtClassTimesStudent> GetClassTimesStudentById(long id);

        /// <summary>
        /// 判断某个班级某个时间点是否有课
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="classId"></param>
        /// <param name="exClassTimesId"></param>
        /// <returns></returns>
        Task<bool> ExistClassTimes(DateTime dateTime, int startTime, int endTime, long classId, long exClassTimesId);

        Task<bool> UpdateClassTimesStudent(long classTimesId, DateTime newClassOt);

        bool AddClassTimesStudent(List<EtClassTimesStudent> etClassTimesStudents);

        Task<long> AddClassTimesStudent(EtClassTimesStudent etClassTimesStudent);

        Task<bool> DelClassTimesStudent(long id);

        Task<Tuple<IEnumerable<EtClassTimes>, int>> GetPaging(RequestPagingBase request);

        Task<IEnumerable<EtClassTimes>> GetList(IValidate request);

        Task<bool> UpdateClassTimesIsClassCheckSign(long classTimesId, long classRecordId, byte newStatus, EtClassRecord record);

        Task<EtClassTimesStudent> GetClassTimesStudent(long classTimesId, long studentTryCalssLogId);

        Task<EtClassTimesStudent> GetClassTimesTryStudent(long studentId, long courseId, DateTime classOt);

        Task<IEnumerable<EtClassTimes>> GetStudentCheckOnAttendClass(DateTime checkOt, long studentId, int relationClassTimesLimitMinuteCard);

        Task SyncClassTimesOfClassTimesRule(EtClassTimesRule rule);

        Task SyncClassTimesReservationType(List<long> classTimesIds, byte newReservationType);

        Task SyncClassTimesReservationLog(EtClassTimes classTimes);

        Task ClassTimesReservationLogAdd(EtClassTimesReservationLog entity);

        Task ClassTimesReservationLogSetCancel(long classTimesId, long studentId);

        Task<int> ClassTimesReservationLogGetCount(long courseId, long studentId, DateTime time);

        Task<Tuple<IEnumerable<EtClassTimesReservationLog>, int>> ReservationLogGetPaging(RequestPagingBase request);

        Task ClassTimesReservationLogEditStatus(long classTimesId, byte newStatus);

        Task ClassTimesReservationLogEditStatusBuyClassCheck(long classTimesId, List<long> inStudentId);

        Task<IEnumerable<ClassTimesClassOtGroupCountView>> ClassTimesClassOtGroupCount(IValidate request);

        Task<IEnumerable<EtClassTimes>> GetClassTimes(IValidate request);
    }
}
