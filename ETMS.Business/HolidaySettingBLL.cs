using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IOC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;

namespace ETMS.Business
{
    public class HolidaySettingBLL : IHolidaySettingBLL
    {
        private readonly IHolidaySettingDAL _holidaySettingDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public HolidaySettingBLL(IHolidaySettingDAL holidaySettingDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._holidaySettingDAL = holidaySettingDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }
        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _holidaySettingDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> HolidaySettingAdd(HolidaySettingAddRequest request)
        {
            await _holidaySettingDAL.AddHolidaySetting(new EtHolidaySetting()
            {
                UserId = request.LoginUserId,
                TenantId = request.LoginTenantId,
                Remark = request.Remark,
                StartTime = request.StartTime.Value,
                EndTime = request.EndTime.Value,
                Ot = DateTime.Now,
                IsDeleted = EmIsDeleted.Normal
            });
            await _userOperationLogDAL.AddUserLog(request, $"设置节假日:{request.Remark}", EmUserOperationType.HolidaySetting);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> HolidaySettingGet(HolidaySettingGetRequest request)
        {
            var holidays = await _holidaySettingDAL.GetAllHolidaySetting();
            return ResponseBase.Success(new GetHolidaySettingOutput()
            {
                Holidays = holidays.Select(p => new HolidaySettingOutputViewOutput()
                {
                    CId = p.Id,
                    Remark = p.Remark,
                    StartTimeDesc = p.StartTime.ToString("yyyy-MM-dd"),
                    EndTimeDesc = p.EndTime.ToString("yyyy-MM-dd")
                }).ToList()
            });
        }


        public async Task<ResponseBase> HolidaySettingDel(HolidaySettingDelRequest request)
        {
            await _holidaySettingDAL.DelHolidaySetting(request.CId);
            await _userOperationLogDAL.AddUserLog(request, "删除节假日设置", EmUserOperationType.HolidaySetting);
            return ResponseBase.Success();
        }
    }
}
