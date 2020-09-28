using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Output;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Database.Source;
using ETMS.Business.Common;

namespace ETMS.Business
{
    public class StatisticsSalesCourseBLL : IStatisticsSalesCourseBLL
    {
        private readonly IStatisticsSalesCourseDAL _statisticsSalesCourseDAL;

        private readonly ICourseDAL _courseDAL;

        public StatisticsSalesCourseBLL(IStatisticsSalesCourseDAL statisticsSalesCourseDAL, ICourseDAL courseDAL)
        {
            this._statisticsSalesCourseDAL = statisticsSalesCourseDAL;
            this._courseDAL = courseDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _statisticsSalesCourseDAL, _courseDAL);
        }

        public async Task StatisticsSalesCourseConsumeEvent(StatisticsSalesCourseEvent request)
        {
            await _statisticsSalesCourseDAL.UpdateStatisticsSalesCourse(request.StatisticsDate.Date);
        }


        public async Task<ResponseBase> GetStatisticsSalesCourseForAmount(GetStatisticsSalesCourseForAmountRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsData = await _statisticsSalesCourseDAL.GetStatisticsSalesCourseForAmount(currentDate, endDate, 20);
            statisticsData = statisticsData.OrderBy(p => p.TotalAmount);
            var output = new EchartsBarVertical<decimal>();
            if (statisticsData != null && statisticsData.Any())
            {
                var tempBox = new DataTempBox<EtCourse>();
                foreach (var item in statisticsData)
                {
                    var courseName = await ComBusiness.GetCourseName(tempBox, _courseDAL, item.CourseId);
                    if (string.IsNullOrEmpty(courseName))
                    {
                        continue;
                    }
                    output.YData.Add(courseName);
                    output.XData.Add(item.TotalAmount);
                }
            }
            return ResponseBase.Success(output);
        }

        public async Task<ResponseBase> GetStatisticsSalesCourseForCount(GetStatisticsSalesCourseForCountRequest request)
        {
            var currentDate = request.StartOt.Value;
            var endDate = request.EndOt.Value;
            var statisticsData = await _statisticsSalesCourseDAL.GetStatisticsSalesCourseForCount(currentDate, endDate, 20, request.BuyUnit);
            statisticsData = statisticsData.OrderBy(p => p.TotalCount);
            var output = new EchartsBarVertical<int>();
            if (statisticsData != null && statisticsData.Any())
            {
                var tempBox = new DataTempBox<EtCourse>();
                foreach (var item in statisticsData)
                {
                    var courseName = await ComBusiness.GetCourseName(tempBox, _courseDAL, item.CourseId);
                    if (string.IsNullOrEmpty(courseName))
                    {
                        continue;
                    }
                    output.YData.Add(courseName);
                    output.XData.Add(item.TotalCount);
                }
            }
            return ResponseBase.Success(output);
        }
    }
}
