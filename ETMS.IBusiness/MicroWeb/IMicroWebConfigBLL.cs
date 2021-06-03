using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Interaction.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETMS.IBusiness.MicroWeb
{
    public interface IMicroWebConfigBLL : IBaseBLL
    {
        Task<ResponseBase> MicroWebBannerGet(RequestBase request);

        Task<ResponseBase> MicroWebBannerSave(MicroWebBannerSaveRequest request);

        Task<List<EtMicroWebColumn>> MicroWebDefaultColumnGet();

        Task<EtMicroWebColumn> MicroWebDefaultColumnGet(long id);

        Task<bool> MicroWebDefaultColumnChangeStatus(int tenantId, long id, byte newStatus);

        Task<bool> MicroWebDefaultColumnSave(MicroWebColumnEditRequest request);

        Task<ResponseBase> MicroWebTenantAddressGet(RequestBase request);

        Task<ResponseBase> MicroWebTenantAddressSave(MicroWebTenantAddressSaveRequest request);
    }
}
