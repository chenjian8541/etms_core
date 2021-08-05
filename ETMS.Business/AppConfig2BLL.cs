using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
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

        public async Task SaveStudentAccountRechargeRule(int tenantId, StudentAccountRechargeRuleView configModel)
        {
            var config = new EtAppConfig()
            {
                ConfigValue = JsonConvert.SerializeObject(configModel),
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                TenantId = tenantId,
                Type = EmAppConfigType.RechargeRuleConfig
            };
            await this._appConfigDAL.SaveAppConfig(config);
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

        public async Task SaveClassReservationSetting(int tenantId, ClassReservationSettingView configModel)
        {
            var config = new EtAppConfig()
            {
                ConfigValue = JsonConvert.SerializeObject(configModel),
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                TenantId = tenantId,
                Type = EmAppConfigType.ClassReservationSetting
            };
            await this._appConfigDAL.SaveAppConfig(config);
        }

        public async Task<List<TeacherSalaryDefaultFundsItemsView>> GetTeacherSalaryDefaultFundsItems()
        {
            var log = await _appConfigDAL.GetAppConfig(EmAppConfigType.TeacherSalaryDefaultFundsItems);
            if (log == null || string.IsNullOrEmpty(log.ConfigValue))
            {
                return TeacherSalaryDefaultFundsItemsView.GetDefaultFundsItems();
            }
            return JsonConvert.DeserializeObject<List<TeacherSalaryDefaultFundsItemsView>>(log.ConfigValue);
        }

        public async Task SaveTeacherSalaryDefaultFundsItems(int tenantId, List<TeacherSalaryDefaultFundsItemsView> entitys)
        {
            await _appConfigDAL.SaveAppConfig(new EtAppConfig()
            {
                ConfigValue = JsonConvert.SerializeObject(entitys),
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                TenantId = tenantId,
                Type = EmAppConfigType.TeacherSalaryDefaultFundsItems
            });
        }

        public async Task<TeacherSalaryGlobalRuleView> GetTeacherSalaryGlobalRule()
        {
            var log = await _appConfigDAL.GetAppConfig(EmAppConfigType.TeacherSalaryGlobalRuleSetting);
            if (log == null || string.IsNullOrEmpty(log.ConfigValue))
            {
                return new TeacherSalaryGlobalRuleView()
                {
                    StatisticalRuleType = EmTeacherSalaryStatisticalRuleType.TotalClassTimesFirst,
                    GradientCalculateType = EmTeacherSalaryGradientCalculateType.None,
                    IncludeArrivedMakeUpStudent = EmBool.True,
                    IncludeArrivedTryCalssStudent = EmBool.False
                };
            }
            return JsonConvert.DeserializeObject<TeacherSalaryGlobalRuleView>(log.ConfigValue);
        }

        public async Task SaveTeacherSalaryGlobalRule(int tenantId, TeacherSalaryGlobalRuleView entity)
        {
            await _appConfigDAL.SaveAppConfig(new EtAppConfig()
            {
                ConfigValue = JsonConvert.SerializeObject(entity),
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                TenantId = tenantId,
                Type = EmAppConfigType.TeacherSalaryGlobalRuleSetting
            });
        }
    }
}
