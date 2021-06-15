using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStatisticsStudentBLL : IBaseBLL
    {
        Task StatisticsStudentCountConsumeEvent(StatisticsStudentCountEvent request);

        Task StatisticsStudentTrackCountConsumeEvent(StatisticsStudentTrackCountEvent request);

        Task StatisticsStudentConsumeEvent(StatisticsStudentEvent request);

        Task<ResponseBase> GetStatisticsStudentCount(GetStatisticsStudentCountRequest request);

        Task<ResponseBase> GetStatisticsStudentTrackCount(GetStatisticsStudentTrackCountRequest request);

        Task<ResponseBase> GetStatisticsStudentSource(GetStatisticsStudentRequest request);

        Task<ResponseBase> GetStatisticsStudentType(GetStatisticsStudentRequest request);

        Task<ResponseBase> GetStatisticsStudentCountPaging(GetStatisticsStudentCountPagingRequest request);

        Task<ResponseBase> GetStatisticsStudentCountMonthPaging(GetStatisticsStudentCountMonthPagingRequest request);

        Task<ResponseBase> GetStatisticsStudentCountMonth(GetStatisticsStudentCountMonthRequest request);
    }
}
