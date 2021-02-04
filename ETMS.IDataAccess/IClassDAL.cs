using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
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

        Task<bool> DelClass(long classId);

        Task<bool> AddClassStudent(EtClassStudent etClassStudent);

        Task<bool> AddClassStudent(List<EtClassStudent> etClassStudents);

        Task<bool> DelClassStudent(long classId, long id);

        Task<bool> DelClassStudentByStudentId(long classId, long studentId);

        Task<long> AddClassTimesRule(long classId, EtClassTimesRule rule);

        bool AddClassTimesRule(long classId, IEnumerable<EtClassTimesRule> rules);

        Task<Tuple<IEnumerable<EtClass>, int>> GetPaging(RequestPagingBase request);

        Task<bool> IsClassCanNotBeDelete(long classId);

        Task<bool> SetClassOverOneToMany(List<long> classIds, DateTime overTime);

        Task<bool> SetClassOverOneToOne(long classId, DateTime overTime);

        Task<bool> IsStudentBuyCourse(long studentId, long courseId);

        Task<bool> IsStudentInClass(long classId, long studentId);

        Task<int> GetClassTimesRuleCount(long classId);

        Task<IEnumerable<EtClassTimesRule>> GetClassTimesRule(long classId, int startTime, int endTime, List<byte> weekDay);

        Task<List<EtClassTimesRule>> GetClassTimesRule(long classId);

        Task<bool> UpdateClassPlanTimes(long classId, byte newScheduleStatus);

        Task<bool> UpdateClassPlanTimes(long classId);

        Task<bool> DelClassTimesRule(long classId, long ruleId);

        Task<bool> ClassEditStudentInfo(long classId, string studentIds, int studentNums);

        Task<bool> SyncClassInfo(long classId, string studentIdsClass, string courseList, string classRoomIds, string teachers, int teacherNum);

        Task<IEnumerable<EtClass>> GetStudentClass(long studentId);

        Task<bool> UpdateClassTeachers(List<long> ids, string teachers, int teacherNum);

        Task RemoveStudent(long studentId);

        Task<List<EtClass>> GetEtClassByOrderId(long orderId);

        Task<IEnumerable<EtClass>> GetClassOfTeacher(long teacherId);

        Task<IEnumerable<EtClass>> GetStudentOneToOneClassNormal(long studentId, long courseId);

        Task<IEnumerable<OnlyClassId>> GetStudentCourseInClass(long studentId, long courseId);
    }
}
