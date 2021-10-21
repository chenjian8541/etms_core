using ETMS.Business.Common;
using ETMS.Entity.Temp;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.BaseBLL
{
    public class TenantLcsAccountBLL
    {
        protected readonly ITenantLcsAccountDAL _tenantLcsAccountDAL;

        protected readonly ISysTenantDAL _sysTenantDAL;

        public TenantLcsAccountBLL(ITenantLcsAccountDAL tenantLcsAccountDAL, ISysTenantDAL sysTenantDAL)
        {
            this._tenantLcsAccountDAL = tenantLcsAccountDAL;
            this._sysTenantDAL = sysTenantDAL;
        }

        protected async Task<CheckTenantLcsAccountView> CheckTenantLcsAccount(int tenantId)
        {
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            var isOpenLcsPay = ComBusiness4.GetIsOpenLcsPay(myTenant.LcswApplyStatus, myTenant.LcswOpenStatus);
            if (!isOpenLcsPay)
            {
                return new CheckTenantLcsAccountView("机构未开通扫呗支付");
            }
            var myLcsAccount = await _tenantLcsAccountDAL.GetTenantLcsAccount(myTenant.Id);
            if (myLcsAccount == null)
            {
                LOG.Log.Error($"[CheckTenantLcsAccount]扫呗账户异常tenantId:{tenantId}", this.GetType());
                return new CheckTenantLcsAccountView("扫呗账户异常，无法支付");
            }
            return new CheckTenantLcsAccountView()
            {
                MyTenant = myTenant,
                MyLcsAccount = myLcsAccount,
                ErrMsg = null
            };
        }
    }
}
