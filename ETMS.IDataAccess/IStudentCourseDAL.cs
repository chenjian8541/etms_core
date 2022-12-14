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
    public interface IStudentCourseDAL : IBaseDAL
    {
        bool AddStudentCourseDetail(List<EtStudentCourseDetail> studentCourseDetails);

        Task AddStudentCourseDetail(EtStudentCourseDetail studentCourseDetail);

        Task<List<EtStudentCourse>> GetStudentCourse(long studentId, long courseId);

        Task<List<long>> GetStudentCourseId(long studentId);

        Task<List<long>> GetStudentCourseIdAboutCanReserve(long studentId);

        Task<List<EtStudentCourse>> GetStudentCourseDb(long studentId, long courseId);

        Task<bool> SetStudentCourseOver(long studentId, long courseId);

        Task<List<EtStudentCourseDetail>> GetStudentCourseDetail(long studentId, long courseId);

        Task<List<EtStudentCourseDetail>> GetStudentCourseDetailStop(long studentId, long courseId);

        Task<List<EtStudentCourseDetail>> GetStudentCourseDetailExLimitTimeAnalysis(long studentId, long courseId);

        Task<IEnumerable<StudentBuyCourse>> GetStudentBuyCourseId(long studentId);

        Task<IEnumerable<StudentBuyCourse>> GetStudentBuyCourseIdIsReading(long studentId);

        Task<bool> EditStudentCourse(long studentId, IEnumerable<EtStudentCourse> courses,
            IEnumerable<EtStudentCourseDetail> details, List<EtStudentCourse> oldStudentCourse, bool isDelOldStudentCourse,
            decimal surplusMoney);

        Task<Tuple<IEnumerable<StudentCourseView>, int>> GetStudentCoursePaging(RequestPagingBase request);

        Task<List<EtStudentCourse>> GetStudentCourse(long studentId);

        Task<List<EtStudentCourseDetail>> GetStudentCourseDetail(long studentId);

        /// <summary>
        /// 保存未购买课程学员的超上课时间
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        Task<bool> SaveNotbuyStudentExceedClassTimes(EtStudentCourse course);

        Task<bool> DeExceedTotalClassTimes(long studentId, long courseId, decimal deExceedTotalClassTimes);

        Task<bool> DeClassTimesOfStudentCourseDetail(long studentCourseDetailId, decimal deClassTimes);

        Task<bool> AddClassTimesOfStudentCourseDetail(long studentCourseDetailId, decimal addClassTimes);

        Task<bool> StudentCourseStop(long studentId, long courseId, DateTime stopTime, DateTime? restoreTime);

        Task<bool> StudentCourseRestoreTime(long studentId, long courseId);

        Task<bool> StudentCourseMarkExceedClassTimes(long studentId, long courseId);

        Task<EtStudentCourseDetail> GetEtStudentCourseDetailById(long id);

        Task<bool> UpdateStudentCourseDetail(EtStudentCourseDetail entity);

        Task<bool> UpdateStudentCourseDetail(List<EtStudentCourseDetail> entitys);

        Task<EtStudentCourseDetail> GetEtStudentCourseDetail(long orderId, long courseId);

        Task<List<EtStudentCourseDetail>> GetStudentCourseDetailByOrderId(long orderId);

        Task StudentMarkGraduation(long studentId);

        Task DelStudentCourseDetailByOrderId(long orderId);

        Task<IEnumerable<StudentCourseNotEnoughNeedRemind>> GetStudentCourseNotEnoughNeedRemind(int studentCourseNotEnoughCount, int limitClassTimes, int limitDay);

        Task UpdateStudentCourseNotEnoughRemindInfo(long studentId, long courseId);

        Task ResetStudentCourseNotEnoughRemindInfo(long studentId, List<long> courseIds);

        Task CancelStudentCourseNotEnoughRemind(long studentId, long courseId);

        Task StudentCourseSetCheckDefault(long studentId, long courseId);

        Task SetStudentCourseDetailNewStatus(long id, long studentId, byte newStatus);

        Task<List<StudentCourseStatusView>> StudentCourseStatusGet(long studentId);

        Task<IEnumerable<OnlyOneFiledCourseId>> StudentStopCourseGet(long studentId);

        Task DelStudentCourse(List<long> ids);

        Task<bool> CheckStudentIsHasEffectiveCourse(long studentId);

        Task ChangeStudentCourseLimitReserve(long student, long courseId, bool isLimitReserve);

        Task<EtStudentCourseExTimeDeLog> GetStudentCourseExTimeDeLog(long studentId, long courseId, long studentCourseDetailId, DateTime compDate);

        Task UpdateStudentCourseDetailEndTime(long id, DateTime newEndTime);

        Task AddStudentCourseExTimeDeLog(EtStudentCourseExTimeDeLog entity);

        Task EditStudentCourseExTimeDeLog(EtStudentCourseExTimeDeLog entity);

        Task UpdateLastDeTime(long studentId,long courseId,DateTime newDeTime);

        Task<EtStudentCourseDetail> GetJustGiveLog(long studentId, long courseId);
    }
}
