using ETMS.Business.Common;
using ETMS.Business.EtmsManage.Common;
using ETMS.Entity.Alien.Dto.Tenant.Output;
using ETMS.Entity.Alien.Dto.Tenant.Request;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.IBusiness.Alien;
using ETMS.IDataAccess.EtmsManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    public class AlienTenantBLL : IAlienTenantBLL
    {
        private readonly ISysTenantOperationLogDAL _sysTenantOperationLogDAL;

        private readonly ISysTenantDAL _sysTenantDAL;

        public AlienTenantBLL(ISysTenantOperationLogDAL sysTenantOperationLogDAL, ISysTenantDAL sysTenantDAL)
        {
            this._sysTenantOperationLogDAL = sysTenantOperationLogDAL;
            this._sysTenantDAL = sysTenantDAL;
        }

        public void InitHeadId(int headId)
        {
        }

        public async Task<ResponseBase> TenantOperationLogPaging(TenantOperationLogPagingRequest request)
        {
            var output = new List<TenantOperationLogPagingOutput>();
            var pagingData = await _sysTenantOperationLogDAL.GetPaging(request);
            if (pagingData.Item1.Any())
            {
                var tempBoxTenant = new AgentDataTempBox<SysTenant>();
                foreach (var p in pagingData.Item1)
                {
                    var tenant = await AgentComBusiness.GetTenant(tempBoxTenant, _sysTenantDAL, p.TenantId);
                    output.Add(new TenantOperationLogPagingOutput()
                    {
                        ClientType = p.ClientType,
                        IpAddress = p.IpAddress,
                        OpContent = p.OpContent,
                        Ot = p.Ot,
                        Type = p.Type,
                        ClientTypeDesc = EmUserOperationLogClientType.GetClientTypeDesc(p.ClientType),
                        TypeDesc = EnumDataLib.GetUserOperationTypeDesc.FirstOrDefault(j => j.Value == p.Type)?.Label,
                        TenantName = tenant?.Name
                    });
                }
            }
            return ResponseBase.Success(new ResponsePagingDataBase<TenantOperationLogPagingOutput>(pagingData.Item2, output));
        }
    }
}
