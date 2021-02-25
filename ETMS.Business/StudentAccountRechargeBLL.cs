using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class StudentAccountRechargeBLL : IStudentAccountRechargeBLL
    {
        private readonly IAppConfigDAL _appConfigDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        public StudentAccountRechargeBLL(IAppConfigDAL appConfigDAL, IUserOperationLogDAL userOperationLogDAL)
        {
            this._appConfigDAL = appConfigDAL;
            this._userOperationLogDAL = userOperationLogDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _appConfigDAL, _userOperationLogDAL);
        }

        public async Task<ResponseBase> StudentAccountRechargeRuleGet(StudentAccountRechargeRuleGetRequest request)
        {
            var log = await this._appConfigDAL.GetAppConfig(EmAppConfigType.RechargeRuleConfig);
            if (log == null)
            {
                return ResponseBase.Success(new StudentAccountRechargeRuleView());
            }
            var rechargeRuleView = JsonConvert.DeserializeObject<StudentAccountRechargeRuleView>(log.ConfigValue);
            return ResponseBase.Success(rechargeRuleView);
        }

        public async Task<ResponseBase> StudentAccountRechargeRuleSave(StudentAccountRechargeRuleSaveRequest request)
        {
            var configModel = new StudentAccountRechargeRuleView()
            {
                Explain = request.Explain,
                ImgUrlKey = request.ImgUrlKey
            };
            var config = new EtAppConfig()
            {
                ConfigValue = JsonConvert.SerializeObject(configModel),
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                TenantId = request.LoginTenantId,
                Type = EmAppConfigType.RechargeRuleConfig
            };
            await this._appConfigDAL.SaveAppConfig(config);

            await _userOperationLogDAL.AddUserLog(request, "充值规则设置", EmUserOperationType.StudentAccountRechargeManage);
            return ResponseBase.Success();
        }
    }
}
