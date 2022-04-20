using ETMS.Entity.Alien.Dto.TenantStatistics.Request;
using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Alien
{
    public interface IAlienTenantStatistics3BLL: IAlienBaseBLL, IAlienTenantBaseBLL
    {
        Task<ResponseBase> AlienTenantStatisticsClassTimesGet(AlienTenantStatisticsClassTimesGetRequest request);

        Task<ResponseBase> AlienTenantStatisticsClassAttendanceTagGet(AlienTenantStatisticsClassAttendanceTagGetRequest request);

        Task<ResponseBase> AlienTenantStatisticsClassAttendanceGet(AlienTenantStatisticsClassAttendanceGetRequest request);

        Task<ResponseBase> AlienTenantStatisticsEducationMonthGet(AlienTenantStatisticsEducationMonthGetRequest request);

        Task<ResponseBase> AlienTenantStatisticsEducationStudentMonthGetPaging(AlienTenantStatisticsEducationStudentMonthGetPagingRequest request);

        Task<ResponseBase> AlienTenantStatisticsEducationTeacherMonthGetPaging(AlienTenantStatisticsEducationTeacherMonthGetPagingRequest request);

        Task<ResponseBase> AlienTenantStatisticsEducationCourseMonthGetPaging(AlienTenantStatisticsEducationCourseMonthGetPagingRequest request);

        Task<ResponseBase> AlienTenantStatisticsEducationClassMonthGetPaging(AlienTenantStatisticsEducationClassMonthGetPagingRequest request);

        Task<ResponseBase> AlienTenantClassRecordGetPaging(AlienTenantClassRecordGetPagingRequest request);

        Task<ResponseBase> AlienTenantClassRecordGet(AlienTenantClassRecordGetRequest request);

        Task<ResponseBase> AlienTenantClassRecordStudentGet(AlienTenantClassRecordStudentGetRequest request);

        Task<ResponseBase> AlienTenantStatisticsWeekGet(AlienTenantStatisticsWeekGetRequest request);

        Task<ResponseBase> AlienTenantStatisticsMonthGet(AlienTenantStatisticsMonthGetRequest request);
    }
}
