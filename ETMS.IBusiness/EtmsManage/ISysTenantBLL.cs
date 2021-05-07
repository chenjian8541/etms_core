using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Entity.EtmsManage.Dto.TenantManage.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysTenantBLL
    {
        ResponseBase TenantNewCodeGet(TenantNewCodeGetRequest request);

        Task<ResponseBase> TenantGetPaging(TenantGetPagingRequest request);

        Task<ResponseBase> TenantGet(TenantGetRequest request);

        Task<ResponseBase> TenantEdit(TenantEditRequest request);

        Task<ResponseBase> TenantSetExDate(TenantSetExDateRequest request);

        Task<ResponseBase> TenantDel(TenantDelRequest request);

        Task<ResponseBase> TenantAdd(TenantAddRequest request);

        Task<ResponseBase> TenantEtmsAccountLogPaging(TenantEtmsAccountLogPagingRequest request);

        Task<ResponseBase> TenantSmsLogPaging(TenantSmsLogPagingRequest request);

        Task<ResponseBase> TenantChangeSms(TenantChangeSmsRequest request);

        Task<ResponseBase> TenantChangeEtms(TenantChangeEtmsRequest request);

        Task<ResponseBase> TenantBindCloudSave(TenantBindCloudSaveRequest request);

        Task<ResponseBase> AIFaceBiduAccountGet(AIFaceBiduAccountGetRequest request);

        Task<ResponseBase> AIFaceBiduAccountGetPaging(AIFaceBiduAccountGetPagingRequest request);

        Task<ResponseBase> AIFaceBiduAccountAdd(AIFaceBiduAccountAddRequest request);

        Task<ResponseBase> AIFaceBiduAccountEdit(AIFaceBiduAccountEditRequest request);

        Task<ResponseBase> AIFaceBiduAccountDel(AIFaceBiduAccountDelRequest request);

        Task<ResponseBase> AIFaceTenantAccountGet(AIFaceTenantAccountGetRequest request);

        Task<ResponseBase> AIFaceTenantAccountGetGetPaging(AIFaceTenantAccountGetGetPagingRequest request);

       Task<ResponseBase> AIFaceTenantAccountAdd(AIFaceTenantAccountAddRequest request);

        Task<ResponseBase> AIFaceTenantAccountEdit(AIFaceTenantAccountEditRequest request);

        Task<ResponseBase> AIFaceTenantAccountDel(AIFaceTenantAccountDelRequest request);

        Task<ResponseBase> AIFaceAllAccountGet(AgentRequestBase request);

        Task<ResponseBase> ResetTenantAdminUserPwd(ResetTenantAdminUserPwdRequest request);
    }
}
