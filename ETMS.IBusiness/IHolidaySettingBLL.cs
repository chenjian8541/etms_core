using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IHolidaySettingBLL : IBaseBLL
    {
        Task<ResponseBase> HolidaySettingAdd(HolidaySettingAddRequest request);

        Task<ResponseBase> HolidaySettingGet(HolidaySettingGetRequest request);

        Task<ResponseBase> HolidaySettingDel(HolidaySettingDelRequest request);
    }
}
