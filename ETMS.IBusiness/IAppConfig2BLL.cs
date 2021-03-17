using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IAppConfig2BLL: IBaseBLL
    {
        Task<StudentAccountRechargeRuleView> GetStudentAccountRechargeRule();

        Task SaveStudentAccountRechargeRule(int tenantId, StudentAccountRechargeRuleView configModel);

        Task<ClassReservationSettingView> GetClassReservationSetting();

        Task SaveClassReservationSetting(int tenantId, ClassReservationSettingView configModel);
    }
}
