﻿using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStatisticsSalesBLL : IBaseBLL
    {
        Task StatisticsSalesProductConsumeEvent(StatisticsSalesProductEvent request);

        Task<ResponseBase> GetStatisticsSalesProduct(GetStatisticsSalesProductRequest request);

        Task<ResponseBase> GetStatisticsSalesProductProportion(GetStatisticsSalesProductProportionRequest request);
    }
}
