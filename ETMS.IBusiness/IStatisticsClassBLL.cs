using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStatisticsClassBLL : IBaseBLL
    {
        Task StatisticsClassConsumeEvent(StatisticsClassEvent request);

        Task StatisticsClassRevokeConsumeEvent(StatisticsClassRevokeEvent request);

        Task StatisticsClassRecordStudentChangeConsumeEvent(StatisticsClassRecordStudentChangeEvent request);

        Task<ResponseBase> StatisticsClassAttendanceGet(StatisticsClassAttendanceRequest request);

        Task<ResponseBase> StatisticsClassTimesGet(StatisticsClassTimesGetRequest request);

        Task<ResponseBase> StatisticsClassCourseGet(StatisticsClassCourseGetRequest request);

        Task<ResponseBase> StatisticsClassTeacherGet(StatisticsClassTeacherGetRequest request);

        Task<ResponseBase> StatisticsClassAttendanceTagGet(StatisticsClassAttendanceTagGetRequest request);

        Task<ResponseBase> StatisticsEducationMonthGet(StatisticsEducationMonthGetRequest request);

        Task<ResponseBase> StatisticsEducationTeacherMonthGetPaging(StatisticsEducationTeacherMonthGetPagingRequest request);

        Task<ResponseBase> StatisticsEducationClassMonthGetPaging(StatisticsEducationClassMonthGetPagingRequest request);

        Task<ResponseBase> StatisticsEducationCourseMonthGetPaging(StatisticsEducationCourseMonthGetPagingRequest request);

        Task<ResponseBase> StatisticsEducationStudentMonthGetPaging(StatisticsEducationStudentMonthGetPagingRequest request);
    }
}
