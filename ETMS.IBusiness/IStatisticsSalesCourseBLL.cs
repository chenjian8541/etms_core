using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStatisticsSalesCourseBLL : IBaseBLL
    {
        Task StatisticsSalesCourseConsumeEvent(StatisticsSalesCourseEvent request);

        Task<ResponseBase> GetStatisticsSalesCourseForAmount(GetStatisticsSalesCourseForAmountRequest request);

        Task<ResponseBase> GetStatisticsSalesCourseForCount(GetStatisticsSalesCourseForCountRequest request);
    }
}
