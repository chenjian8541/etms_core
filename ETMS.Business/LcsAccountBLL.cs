using ETMS.Business.BaseBLL;
using ETMS.Entity.Temp;
using ETMS.IBusiness;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class LcsAccountBLL : TenantLcsAccountBLL, ILcsAccountBLL
    {
        public LcsAccountBLL(ITenantLcsAccountDAL tenantLcsAccountDAL, ISysTenantDAL sysTenantDAL)
            : base(tenantLcsAccountDAL, sysTenantDAL)
        {
        }

        public void InitTenantId(int tenantId)
        {
        }

        public async Task<CheckTenantLcsAccountView> CheckLcsAccount(int tenantId)
        {
            return await base.CheckTenantLcsAccount(tenantId);
        }
    }
}
