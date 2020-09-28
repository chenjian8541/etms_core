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

        Task<ResponseBase> StudentCourseDetailGet(StudentCourseDetailGetRequest request);

        Task<ResponseBase> StudentCourseStop(StudentCourseStopRequest request);

        Task<ResponseBase> StudentCourseRestoreTime(StudentCourseRestoreTimeRequest request);

        Task<ResponseBase> StudentCourseMarkExceedClassTimes(StudentCourseMarkExceedClassTimesRequest request);

        Task<ResponseBase> StudentCourseSetExpirationDate(StudentCourseSetExpirationDateRequest request);

        Task<ResponseBase> StudentCourseClassOver(StudentCourseClassOverRequest request);

        Task<ResponseBase> StudentCourseChangeTimes(StudentCourseChangeTimesRequest request);

        Task<ResponseBase> StudentCourseSurplusGet(StudentCourseSurplusGetRequest request);

        Task<ResponseBase> StudentCourseConsumeLogGetPaging(StudentCourseConsumeLogGetPagingRequest request);
    }
}
