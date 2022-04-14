using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.Head.Request;
using ETMS.IBusiness.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface IHeadBLL: IAlienBaseBLL
    {
        Task<ResponseBase> HeadGetSimple(HeadGetRequest request);

        Task<ResponseBase> HeadGet(HeadGetRequest request);

        Task<ResponseBase> HeadGetPaging(HeadGetPagingRequest request);

        Task<ResponseBase> HeadAdd(HeadAddRequest request);

        Task<ResponseBase> HeadEdit(HeadEditRequest request);

        Task<ResponseBase> HeadDel(HeadDelRequest request);

        Task<ResponseBase> HeadAddTenant(HeadAddTenantRequest request);

        Task<ResponseBase> HeadRemoveTenant(HeadRemoveTenantRequest request);
    }
}
