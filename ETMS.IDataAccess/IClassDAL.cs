using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.Entity.View.OnlyOneFiled;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IClassDAL : IBaseDAL
    {
        Task<ClassBucket> GetClassBucket(long classId);

        Task<long> AddClass(EtClass etClass);

        Task<bool> EditClass(EtClass etClass);

        Task UpdateReservationInfo(List<long> classIds, byte newReservationType, int newDurationHour, int newDurationMinute,int newDunIntervalMinute);

        Task<bool> DelClass(long classId, bool isIgnoreCheck = false);

        Task<IEnumerable<OnlyOneFiledDateTime>> GetClassRecordAllDate(long classId);

        Task<IEnumerable<ClassRecordTeacherInfoView>> GetClassRecordTeacherInfoView(long classId);

        Task<bool> DelClassDepth(long classId);

        Task<bool> AddClassStudent(EtClassStudent etClassStudent);

        Task<bool> AddClassStudent(List<EtClassStudent> etClassStudents);

        Task<bool> DelClassStudent(long classId, long id);

        Task<bool> DelClassStudentByStudentId(long classId, long studentId);

        Task<long> AddClassTimesRule(long classId, EtClassTimesRule rule);

        Task<EtClassTimesRule> GetClassTimesRuleBuyId(long id);

        Task EditClassTimesRule(EtClassTimesRule rule);

        bool AddClassTimesRule(long classId, IEnumerable<EtClassTimesRule> rules);

        Task<Tuple<IEnumerable<EtClass>, int>> GetPaging(IPagingRequest request);

        Task<bool> IsClassCanNotBeDelete(long classId);

        Task<bool> SetClassOverOneToMany(List<long> classIds, DateTime overTime);

        Task<bool> SetClassOverOneToOne(long classId, DateTime overTime);

        Task<bool> IsStudentBuyCourse(long studentId, long courseId);

        Task<bool> IsStudentInClass(long classId, long studentId);

        Task<int> GetClassTimesRuleCount(long classId);

        Task<IEnumerable<EtClassTimesRule>> GetClassTimesRule(long classId, int startTime, int endTime, List<byte> weekDay);

        Task<IEnumerable<EtClassTimes>> GetClassTimesRuleTeacher(long teacherId, int startTime,
           int endTime, List<byte> weekDay, DateTime startDate, DateTime? endDate, int topCount, long excRuleId = 0);

        Task<IEnumerable<EtClassTimes>> GetClassTimesRuleStudent(long studentId, int startTime,
           int endTime, List<byte> weekDay, DateTime startDate, DateTime? endDate, int topCount, long excRuleId = 0);

        Task<IEnumerable<EtClassTimes>> GetClassTimesRuleClassRoom(long classRoomId, int startTime,
            int endTime, List<byte> weekDay, DateTime startDate, DateTime? endDate, int topCount, long excRuleId = 0);

        Task<List<EtClassTimesRule>> GetClassTimesRule(long classId);

        Task<bool> UpdateClassPlanTimes(long classId, byte newScheduleStatus);

        Task<bool> UpdateClassPlanTimes(long classId);

        Task<bool> DelClassTimesRule(long classId, long ruleId);

        Task<bool> ClassEditStudentInfo(long classId, string studentIds, int studentNums);

        Task SyncClassStudentInfo(long classId, string studentIdsClass, string courseList, string classRoomIds,
            string teachers, int teacherNum, int? limitStudentNums, int limitStudentNumsType, int studentClassCount);

        List<string> GetSyncClassInfoSql(long classId, string studentIdsClass, string courseList, string classRoomIds,
            string teachers, int teacherNum, int? limitStudentNums, int limitStudentNumsType, int studentClassCount);

        Task<IEnumerable<EtClass>> GetStudentClass(long studentId);

        Task<bool> UpdateClassTeachers(List<long> ids, string teachers, int teacherNum);

        Task<IEnumerable<OnlyClassId>> RemoveStudent(long studentId);

        Task<List<EtClass>> GetEtClassByOrderId(long orderId);

        Task<IEnumerable<EtClass>> GetClassOfTeacher(long teacherId);

        Task<IEnumerable<EtClass>> GetClassOfTeacher2(long teacherId);

        Task<IEnumerable<EtClass>> GetClassOfCourseIdOneToMore(long courseId, string queryClassName = "");

        Task<IEnumerable<EtClass>> GetStudentOneToOneClassNormal(long studentId, long courseId);

        Task<IEnumerable<OnlyClassId>> GetStudentCourseInClass(long studentId, long courseId);

        Task<IEnumerable<EtClassStudent>> GetStudentClass2(long studentId);

        Task<bool> UpdateClassFinishInfo(long classId, int finishCount, decimal finishClassTimes);

        Task ChangeClassOnlineSelClassStatus(long classId, byte newIsCanOnlineSelClass);

        Task ChangeClassOnlineSelClassStatus(List<long> ids, byte newIsCanOnlineSelClass);

        Task<IEnumerable<ClassCanChooseView>> GetStudentClassCanChoose(long studentId, long courseId);

        Task<IEnumerable<EtClass>> GetStudentOneToOneClassNormalIsReservation(long studentId);

        Task<bool> CheckStudentHaveOneToOneClassNormalIsReservation(long studentId);

        Task UpdateClassTimesRuleDataType(long id, byte newDataType);

        Task UpdateClassTimesRuleDataType(List<long> ids, byte newDataType);

        Task<EtClassStudent> GetClassStudentById(long id);
    }
}
