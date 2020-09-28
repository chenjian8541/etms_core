using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStatisticsFinanceBLL: IBaseBLL
    {
        Task StatisticsFinanceIncomeConsumeEvent(StatisticsFinanceIncomeEvent request);

        Task<ResponseBase> GetStatisticsFinanceIn(GetStatisticsFinanceInRequest request);

        Task<ResponseBase> GetStatisticsFinanceInProjectType(GetStatisticsFinanceInProjectTypeRequest request);

        Task<ResponseBase> GetStatisticsFinanceInPayType(GetStatisticsFinanceInPayTypeRequest request);

        Task<ResponseBase> GetStatisticsFinanceOut(GetStatisticsFinanceOutRequest request);

        Task<ResponseBase> GetStatisticsFinanceOutProjectType(GetStatisticsFinanceOutProjectTypeRequest request);

        Task<ResponseBase> GetStatisticsFinanceOutPayType(GetStatisticsFinanceOutPayTypeRequest request);
    }
}
