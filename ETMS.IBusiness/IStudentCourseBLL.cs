using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentCourseBLL : IBaseBLL
    {
        Task<ResponseBase> StudentCourseGetPaging(StudentCourseGetPagingRequest request);

        Task<ResponseBase> StudentCourseOwnerGetPaging(StudentCourseOwnerGetPagingRequest request);

        Task<ResponseBase> StudentCourseDetailGet(StudentCourseDetailGetRequest request);

        Task<ResponseBase> StudentCourseDetailGet2(StudentCourseDetailGetRequest request);

        Task<ResponseBase> StudentCourseStop(StudentCourseStopRequest request);

        Task<ResponseBase> StudentCourseStopBatch(StudentCourseStopBatchRequest request);

        Task<ResponseBase> StudentCourseRestoreTime(StudentCourseRestoreTimeRequest request);

        Task<ResponseBase> StudentCourseRestoreTimeBatch(StudentCourseRestoreTimeBatchRequest request);

        Task<ResponseBase> StudentCourseMarkExceedClassTimes(StudentCourseMarkExceedClassTimesRequest request);

        Task<ResponseBase> StudentCourseSetExpirationDate(StudentCourseSetExpirationDateRequest request);

        Task<ResponseBase> StudentClearance(StudentCourseClearRequest request);

        Task<ResponseBase> StudentCourseClassOver(StudentCourseClassOverRequest request);

        Task<ResponseBase> StudentCourseChangeTimes(StudentCourseChangeTimesRequest request);

        Task<ResponseBase> StudentCourseSurplusGet(StudentCourseSurplusGetRequest request);

        Task<ResponseBase> StudentCourseConsumeLogGetPaging(StudentCourseConsumeLogGetPagingRequest request);

        /// <summary>
        /// 获取学员课程信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ResponseBase> StudentCourseHasGet(StudentCourseHasGetRequest request);

        /// <summary>
        /// 获取学员课程详情信息（用于转课）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ResponseBase> StudentCourseHasDetailGet(StudentCourseHasDetailGetRequest request);

        /// <summary>
        /// 学员课时不足提醒
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ResponseBase StudentCourseNotEnoughRemind(StudentCourseNotEnoughRemindRequest request);

        Task<ResponseBase> StudentCourseNotEnoughRemindCancel(StudentCourseNotEnoughRemindCancelRequest request);

        Task<ResponseBase> StudentCourseSetCheckDefault(StudentCourseSetCheckDefaultRequest request);
    }
}
