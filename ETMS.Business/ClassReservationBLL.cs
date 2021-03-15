using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Output;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class ClassReservationBLL : IClassReservationBLL
    {
        private readonly IAppConfig2BLL _appConfig2BLL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public ClassReservationBLL(IAppConfig2BLL appConfig2BLL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._appConfig2BLL = appConfig2BLL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this._appConfig2BLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _userOperationLogDAL);
        }

        public async Task<ResponseBase> ClassReservationRuleGet(RequestBase request)
        {
            var rule = await _appConfig2BLL.GetClassReservationSetting();
            return ResponseBase.Success(new ClassReservationRuleGetView()
            {
                DeadlineClassReservaLimitDayTimeValue = rule.DeadlineClassReservaLimitDayTimeValue,
                DeadlineClassReservaLimitDayTimeValueDesc = EtmsHelper.GetTimeDesc(rule.DeadlineClassReservaLimitDayTimeValue),
                DeadlineClassReservaLimitType = rule.DeadlineClassReservaLimitType,
                DeadlineClassReservaLimitValue = rule.DeadlineClassReservaLimitValue,
                IsParentShowClassCount = rule.IsParentShowClassCount,
                MaxCountClassReservaLimitType = rule.MaxCountClassReservaLimitType,
                StartClassReservaLimitType = rule.StartClassReservaLimitType,
                StartClassReservaLimitValue = rule.StartClassReservaLimitValue,
                MaxCountClassReservaLimitValue = rule.MaxCountClassReservaLimitValue
            });
        }

        public async Task<ResponseBase> ClassReservationRuleSave(ClassReservationRuleSaveRequest request)
        {
            var rule = await _appConfig2BLL.GetClassReservationSetting();
            rule.DeadlineClassReservaLimitDayTimeValue = request.DeadlineClassReservaLimitDayTimeValue;
            rule.DeadlineClassReservaLimitType = request.DeadlineClassReservaLimitType;
            rule.DeadlineClassReservaLimitValue = request.DeadlineClassReservaLimitValue;
            rule.IsParentShowClassCount = request.IsParentShowClassCount;
            rule.MaxCountClassReservaLimitType = request.MaxCountClassReservaLimitType;
            rule.StartClassReservaLimitType = request.StartClassReservaLimitType;
            rule.StartClassReservaLimitValue = request.StartClassReservaLimitValue;
            rule.MaxCountClassReservaLimitValue = request.MaxCountClassReservaLimitValue;
            return ResponseBase.Success();
        }
    }
}
