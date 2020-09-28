using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
using ETMS.IBusiness;
using ETMS.IBusiness.EtmsManage;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EtmsManage
{
    public class SysTenantManage : ISysTenantManage
    {
        private readonly IEtmsSourceDAL _etmsSourceDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        public SysTenantManage(IEtmsSourceDAL etmsSourceDAL, ISysTenantDAL sysTenantDAL)
        {
            this._etmsSourceDAL = etmsSourceDAL;
            this._sysTenantDAL = sysTenantDAL;
        }
        public async Task<ResponseBase> TenantAdd(TenantAddRequest request)
        {
            var hisData = await _sysTenantDAL.GetTenant(request.TenantCode);
            if (hisData != null)
            {
                return ResponseBase.CommonError("机构编码已存在");
            }
            var tenantId = await _sysTenantDAL.AddTenant(new SysTenant()
            {
                ConnectionId = request.ConnectionId,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.TenantName,
                Ot = DateTime.Now,
                Phone = request.TenantPhone,
                Remark = request.Remark,
                TenantCode = request.TenantCode
            });
            _etmsSourceDAL.InitTenantId(tenantId);
            _etmsSourceDAL.InitEtmsSourceData(tenantId, request.TenantName, request.UserName, request.TenantPhone);
            return ResponseBase.Success();
        }
    }
}
