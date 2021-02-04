using ETMS.DataAccess.Core;
using ETMS.Entity.CacheBucket;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.DataAccess
{
    public class StudentCourseDAL : DataAccessBase<StudentCourseBucket>, IStudentCourseDAL
    {
        public StudentCourseDAL(IDbWrapper dbWrapper, ICacheProvider cacheProvider) : base(dbWrapper, cacheProvider)
        {
        }

        protected override async Task<StudentCourseBucket> GetDb(params object[] keys)
        {
            var studentCourses = await _dbWrapper.FindList<EtStudentCourse>(p => p.TenantId == _tenantId && p.StudentId == keys[1].ToLong()
            && p.IsDeleted == EmIsDeleted.Normal);
            return new StudentCourseBucket()
            {
                StudentCourses = studentCourses
            };
        }

        public bool AddStudentCourseDetail(List<EtStudentCourseDetail> studentCourseDetails)
        {
            return _dbWrapper.InsertRange(studentCourseDetails);
        }

        public async Task<List<EtStudentCourse>> GetStudentCourse(long studentId, long courseId)
        {
            var allCourse = await base.GetCache(_tenantId, studentId);
            return allCourse.StudentCourses.Where(p => p.CourseId == courseId).ToList();
        }

        public async Task<List<EtStudentCourse>> GetStudentCourseDb(long studentId, long courseId)
        {
            return await _dbWrapper.FindList<EtStudentCourse>(p => p.TenantId == _tenantId && p.StudentId == studentId
            && p.CourseId == courseId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<bool> SetStudentCourseOver(long studentId, long courseId)
        {
            var sql = $"UPDATE EtStudentCourse SET [Status] = {EmStudentCourseStatus.StopOfClass} WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId} AND IsDeleted = {EmIsDeleted.Normal} ";
            await _dbWrapper.Execute(sql);
            await UpdateCache(_tenantId, studentId);
            return true;
        }

        public async Task<List<EtStudentCourseDetail>> GetStudentCourseDetail(long studentId, long courseId)
        {
            return await _dbWrapper.FindList<EtStudentCourseDetail>(p => p.TenantId == _tenantId && p.StudentId == studentId
            && p.IsDeleted == EmIsDeleted.Normal && p.CourseId == courseId);
        }

        public async Task<IEnumerable<StudentBuyCourse>> GetStudentBuyCourseId(long studentId)
        {
            return await _dbWrapper.ExecuteObject<StudentBuyCourse>($"SELECT DISTINCT CourseId FROM EtStudentCourseDetail WHERE TenantId = {_tenantId} AND StudentId = {studentId} ");
        }

        public async Task<bool> EditStudentCourse(long studentId, IEnumerable<EtStudentCourse> courses,
            IEnumerable<EtStudentCourseDetail> details, List<EtStudentCourse> oldStudentCourse, bool isDelOldStudentCourse)
        {
            if (isDelOldStudentCourse && courses.Count() == 0 && oldStudentCourse.Count > 0)
            {
                foreach (var p in oldStudentCourse)
                {
                    p.IsDeleted = EmIsDeleted.Deleted;
                    await _dbWrapper.Update(p);
                }
            }
            foreach (var c in courses)
            {
                if (c.Id > 0)
                {
                    await _dbWrapper.Update(c);
                }
                else
                {
                    await _dbWrapper.Insert(c);
                }
            }
            await UpdateCache(_tenantId, studentId);
            return await _dbWrapper.UpdateRange(details);
        }

        public async Task<Tuple<IEnumerable<StudentCourseView>, int>> GetStudentCoursePaging(RequestPagingBase request)
        {
            return await _dbWrapper.ExecutePage<StudentCourseView>("StudentCourseView", "*", request.PageSize, request.PageCurrent, "Id DESC", request.ToString());
        }

        public async Task<List<EtStudentCourse>> GetStudentCourse(long studentId)
        {
            return (await GetCache(_tenantId, studentId)).StudentCourses;
        }

        public async Task<List<EtStudentCourseDetail>> GetStudentCourseDetail(long studentId)
        {
            return await _dbWrapper.FindList<EtStudentCourseDetail>(p => p.TenantId == _tenantId && p.StudentId == studentId && p.IsDeleted == EmIsDeleted.Normal);
        }

        /// <summary>
        /// 保存未购买课程学员的超上课时间
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public async Task<bool> SaveNotbuyStudentExceedClassTimes(EtStudentCourse course)
        {
            var historyLog = await _dbWrapper.Find<EtStudentCourse>(
                p => p.DeType == course.DeType && p.TenantId == course.TenantId
                && p.StudentId == course.StudentId && p.CourseId == course.CourseId
                && p.IsDeleted == EmIsDeleted.Normal);
            if (historyLog == null)
            {
                await _dbWrapper.Insert(course);
            }
            else
            {
                await _dbWrapper.Execute($"UPDATE EtStudentCourse SET ExceedTotalClassTimes = ExceedTotalClassTimes + {course.ExceedTotalClassTimes} WHERE id = {historyLog.Id}");
            }
            return true;
        }

        /// <summary>
        /// 扣减超上课时(撤销点名使用)
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        /// <param name="deExceedTotalClassTimes"></param>
        /// <returns></returns>
        public async Task<bool> DeExceedTotalClassTimes(long studentId, long courseId, decimal deExceedTotalClassTimes)
        {
            var historyLog = await _dbWrapper.Find<EtStudentCourse>(
                p => p.DeType == EmDeClassTimesType.ClassTimes
                && p.TenantId == _tenantId
                && p.StudentId == studentId
                && p.CourseId == courseId
                && p.IsDeleted == EmIsDeleted.Normal);
            if (historyLog != null)
            {
                if (historyLog.ExceedTotalClassTimes >= deExceedTotalClassTimes)
                {
                    await _dbWrapper.Execute($"UPDATE EtStudentCourse SET ExceedTotalClassTimes = ExceedTotalClassTimes - {deExceedTotalClassTimes} WHERE id = {historyLog.Id}");
                }
                else
                {
                    await _dbWrapper.Execute($"UPDATE EtStudentCourse SET ExceedTotalClassTimes = 0 WHERE id = {historyLog.Id}");
                }
            }
            return true;
        }

        /// <summary>
        /// 扣课时
        /// </summary>
        /// <param name="studentCourseDetailId"></param>
        /// <param name="deClassTimes"></param>
        /// <returns></returns>
        public async Task<bool> DeClassTimesOfStudentCourseDetail(long studentCourseDetailId, decimal deClassTimes)
        {
            var count = await _dbWrapper.Execute($"UPDATE EtStudentCourseDetail SET SurplusQuantity = SurplusQuantity - {deClassTimes},UseQuantity = UseQuantity + {deClassTimes} WHERE id = {studentCourseDetailId} ");
            return count > 0;
        }

        /// <summary>
        /// 增加课时(撤销点名使用)
        /// </summary>
        /// <param name="studentCourseDetailId"></param>
        /// <param name="addClassTimes"></param>
        /// <returns></returns>
        public async Task<bool> AddClassTimesOfStudentCourseDetail(long studentCourseDetailId, decimal addClassTimes)
        {
            var count = await _dbWrapper.Execute($"UPDATE EtStudentCourseDetail SET SurplusQuantity = SurplusQuantity + {addClassTimes},UseQuantity = UseQuantity - {addClassTimes} WHERE id = {studentCourseDetailId} ");
            return count > 0;
        }

        /// <summary>
        /// 停课
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        /// <param name="stopTime"></param>
        /// <returns></returns>
        public async Task<bool> StudentCourseStop(long studentId, long courseId, DateTime stopTime)
        {
            var sql = new StringBuilder($"UPDATE EtStudentCourse SET [Status] = {EmStudentCourseStatus.StopOfClass} ,StopTime = '{stopTime.EtmsToDateString()}',RestoreTime = NULL WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] <> {EmStudentCourseStatus.EndOfClass} ;");
            sql.Append($"UPDATE EtStudentCourseDetail SET [Status] = {EmStudentCourseStatus.StopOfClass} WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] <> {EmStudentCourseStatus.EndOfClass} ;");
            var count = await _dbWrapper.Execute(sql.ToString());
            await UpdateCache(_tenantId, studentId);
            return count > 0;
        }

        /// <summary>
        /// 复课
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public async Task<bool> StudentCourseRestoreTime(long studentId, long courseId)
        {
            var sql = new StringBuilder($"UPDATE EtStudentCourse SET [Status] = {EmStudentCourseStatus.Normal} ,StopTime = NULL,RestoreTime = NULL WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmStudentCourseStatus.StopOfClass} ;");
            sql.Append($"UPDATE EtStudentCourseDetail SET [Status] = {EmStudentCourseStatus.Normal} WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmStudentCourseStatus.StopOfClass} ;");
            var count = await _dbWrapper.Execute(sql.ToString());
            await UpdateCache(_tenantId, studentId);
            return count > 0;
        }

        public async Task<bool> StudentCourseMarkExceedClassTimes(long studentId, long courseId)
        {
            var sql = $"UPDATE EtStudentCourse SET ExceedTotalClassTimes = 0 WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId} AND IsDeleted = {EmIsDeleted.Normal} ;";
            var count = await _dbWrapper.Execute(sql);
            await UpdateCache(_tenantId, studentId);
            return count > 0;
        }

        public async Task<EtStudentCourseDetail> GetEtStudentCourseDetailById(long id)
        {
            return await _dbWrapper.Find<EtStudentCourseDetail>(id);
        }

        public async Task<bool> UpdateStudentCourseDetail(EtStudentCourseDetail entity)
        {
            await _dbWrapper.Update(entity);
            await UpdateCache(_tenantId, entity.StudentId);
            return true;
        }

        public async Task<bool> UpdateStudentCourseDetail(List<EtStudentCourseDetail> entitys)
        {
            await _dbWrapper.UpdateRange(entitys);
            await UpdateCache(_tenantId, entitys[0].StudentId);
            return true;
        }

        public async Task<EtStudentCourseDetail> GetEtStudentCourseDetail(long orderId, long courseId)
        {
            return await _dbWrapper.Find<EtStudentCourseDetail>(p => p.OrderId == orderId && p.CourseId == courseId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task<List<EtStudentCourseDetail>> GetStudentCourseDetailByOrderId(long orderId)
        {
            return await _dbWrapper.FindList<EtStudentCourseDetail>(p => p.OrderId == orderId && p.IsDeleted == EmIsDeleted.Normal);
        }

        public async Task StudentMarkGraduation(long studentId)
        {
            var sql = new StringBuilder();
            sql.Append($"UPDATE EtStudentCourse SET [Status] = {EmStudentCourseStatus.EndOfClass} WHERE TenantId = {_tenantId} AND StudentId = {studentId};");
            sql.Append($"UPDATE EtStudentCourseDetail SET [Status] = {EmStudentCourseStatus.EndOfClass} WHERE TenantId = {_tenantId} AND StudentId = {studentId};");
            await _dbWrapper.Execute(sql.ToString());
        }

        public async Task DelStudentCourseDetailByOrderId(long orderId)
        {
            var sql = $"UPDATE EtStudentCourseDetail SET IsDeleted = {EmIsDeleted.Deleted} WHERE TenantId = {_tenantId} AND OrderId = {orderId} ";
            await _dbWrapper.Execute(sql);
        }

        public async Task<IEnumerable<StudentCourseNotEnoughNeedRemind>> GetStudentCourseNotEnoughNeedRemind(int studentCourseNotEnoughCount, int limitClassTimes, int limitDay)
        {
            return await _dbWrapper.ExecuteObject<StudentCourseNotEnoughNeedRemind>(
                $"SELECT TOP 200 StudentId,CourseId  from StudentCourseView WHERE TenantId = {_tenantId} AND IsDeleted = {EmIsDeleted.Normal} AND BuyQuantity > 0 AND StudentType = {EmStudentType.ReadingStudent} AND NotEnoughRemindCount < {studentCourseNotEnoughCount} AND ((DeType={EmDeClassTimesType.ClassTimes} AND SurplusQuantity <= {limitClassTimes}) OR (DeType<>{EmDeClassTimesType.ClassTimes} AND SurplusQuantity=0 AND SurplusSmallQuantity <={limitDay})) GROUP BY StudentId,CourseId");
        }

        public async Task UpdateStudentCourseNotEnoughRemindInfo(long studentId, long courseId)
        {
            await this._dbWrapper.Execute($"UPDATE EtStudentCourse SET NotEnoughRemindCount = NotEnoughRemindCount + 1,NotEnoughRemindLastTime = '{DateTime.Now.EtmsToString()}' WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseId}");
            await UpdateCache(_tenantId, studentId);
        }

        public async Task ResetStudentCourseNotEnoughRemindInfo(long studentId, List<long> courseIds)
        {
            var sql = string.Empty;
            if (courseIds.Count == 1)
            {
                sql = $"UPDATE EtStudentCourse SET NotEnoughRemindCount = 0 WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId = {courseIds[0]} ";
            }
            else
            {
                sql = $"UPDATE EtStudentCourse SET NotEnoughRemindCount = 0 WHERE TenantId = {_tenantId} AND StudentId = {studentId} AND CourseId IN ({string.Join(',', courseIds)})";
            }
            await _dbWrapper.Execute(sql);
        }
    }
}
