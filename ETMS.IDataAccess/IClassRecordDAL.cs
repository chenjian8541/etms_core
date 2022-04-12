using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp.View;
using ETMS.Entity.View;
using ETMS.Entity.View.OnlyOneFiled;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IClassRecordDAL : IBaseDAL
    {
        Task<long> AddEtClassRecord(EtClassRecord etClassRecord, List<EtClassRecordStudent> classRecordStudents, List<EtClassRecordEvaluateStudent> evaluateStudents = null);

        Task<EtClassRecordAbsenceLog> GetRelatedAbsenceLog(long studentId, long courseId);

        void AddClassRecordAbsenceLog(List<EtClassRecordAbsenceLog> classRecordAbsenceLogs);

        void AddClassRecordPointsApplyLog(List<EtClassRecordPointsApplyLog> classRecordPointsApplyLog);

        Task<Tuple<IEnumerable<EtClassRecord>, int>> GetPaging(IPagingRequest request);

        Task<EtClassRecord> GetClassRecord(long classRecordId);

        Task<List<EtClassRecordStudent>> GetClassRecordStudents(long classRecordId);

        Task<Tuple<IEnumerable<ClassRecordAbsenceLogView>, int>> GetClassRecordAbsenceLogPaging(RequestPagingBase request);

        Task<EtClassRecordAbsenceLog> GetClassRecordAbsenceLog(long id);

        Task<List<EtClassRecordAbsenceLog>> GetClassRecordAbsenceLogByClassRecordId(long classRecordId);

        Task<bool> UpdateClassRecordAbsenceLog(EtClassRecordAbsenceLog log);

        Task<EtClassRecordPointsApplyLog> GetClassRecordPointsApplyLog(long id);

        Task<bool> EditClassRecordPointsApplyLog(EtClassRecordPointsApplyLog log);

        Task<Tuple<IEnumerable<EtClassRecordStudent>, int>> GetClassRecordStudentPaging(IPagingRequest request);

        Task<EtClassRecordStudent> GetEtClassRecordStudentById(long id);

        Task<bool> EditClassRecord(EtClassRecord classRecord);

        Task<bool> EditClassRecordStudent(EtClassRecordStudent etClassRecordStudent, bool isChangeDeClassTime = false);

        Task<Tuple<IEnumerable<ClassRecordPointsApplyLogView>, int>> GetClassRecordPointsApplyLog(RequestPagingBase request);

        Task<List<EtClassRecordPointsApplyLog>> GetClassRecordPointsApplyLogByClassRecordId(long classRecordId);

        Task<bool> SetClassRecordRevoke(long classRecordId);

        Task<bool> ClassRecordAddEvaluateStudentCount(long classRecordId, int addCount);

        Task<bool> AddClassRecordOperationLog(EtClassRecordOperationLog log);

        Task<Tuple<IEnumerable<EtClassRecordOperationLog>, int>> GetClassRecordOperationLogPaging(RequestPagingBase request);

        Task<EtClassRecordPointsApplyLog> GetClassRecordPointsApplyLogByClassRecordId(long classRecordId, long studentId);

        Task<ClassRecordStatistics> GetClassRecordStatistics(long classId);

        Task<bool> ClassRecordStudentDeEvaluateCount(long classRecordStudentId, int deCount);

        Task<List<EtClassRecord>> GetClassRecord(DateTime classOt);

        Task<bool> ExistClassRecord(long classId, DateTime classOt, int startTime, int endTime);

        Task<ClassRecordTeacherStatistics> GetClassRecordTeacherStatistics(long teacherId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 学员期间各课程的请假次数
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="courseIds"></param>
        /// <returns></returns>
        Task<IEnumerable<StudentCourseIsLeaveCountView>> GetClassRecordStudentCourseIsLeaveCount(long studentId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 获取超上课时未处理的记录
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        Task<List<EtClassRecordStudent>> ClassRecordStudentHasUntreatedExceed(long studentId, long courseId);

        Task UpdateClassRecordStudentIsExceedProcessed(long studentId, long courseId);

        Task ClassRecordAddDeSum(long id, decimal addDeSum);

        Task SyncClassCategoryId(long classId, long? classCategoryId);

        Task UpdateClassRecordStudentSurplusCourseDesc(List<UpdateStudentLogOfSurplusCourseView> upLogs);
    }
}
