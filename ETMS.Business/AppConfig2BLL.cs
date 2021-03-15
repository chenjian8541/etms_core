using ETMS.Entity.Common;
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
    public class AppConfig2BLL : IAppConfig2BLL
    {
        private readonly IAppConfigDAL _appConfigDAL;

        public AppConfig2BLL(IAppConfigDAL appConfigDAL)
        {
            this._appConfigDAL = appConfigDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _appConfigDAL);
        }

        public async Task<StudentAccountRechargeRuleView> GetStudentAccountRechargeRule()
        {
            var log = await this._appConfigDAL.GetAppConfig(EmAppConfigType.RechargeRuleConfig);
            if (log == null)
            {
                return new StudentAccountRechargeRuleView();
            }
            var rechargeRuleView = JsonConvert.DeserializeObject<StudentAccountRechargeRuleView>(log.ConfigValue);
            return rechargeRuleView;
        }

        public async Task<ClassReservationSettingView> GetClassReservationSetting()
        {
            var log = await this._appConfigDAL.GetAppConfig(EmAppConfigType.ClassReservationSetting);
            if (log == null)
            {
                return new ClassReservationSettingView();
            }
            var classReservationSettingView = JsonConvert.DeserializeObject<ClassReservationSettingView>(log.ConfigValue);
            return classReservationSettingView;
        }
    }
}
