using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.IBusiness;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class TenantBLL : ITenantBLL
    {
        private readonly ISysTenantDAL _sysTenantDAL;


        public TenantBLL(ISysTenantDAL sysTenantDAL, ISysVersionDAL sysVersionDAL)
        {
            this._sysTenantDAL = sysTenantDAL;
        }

        public void InitTenantId(int tenantId)
        {
        }

        public async Task<ResponseBase> TenantGet(TenantGetRequest request)
        {
            var myTenant = await _sysTenantDAL.GetTenant(request.LoginTenantId);
            return ResponseBase.Success(myTenant);
        }
    }
}
