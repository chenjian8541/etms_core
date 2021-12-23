using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using ETMS.Entity.Dto.ConfigMgr.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IShareTemplateBLL : IBaseBLL
    {
        Task<ResponseBase> ShareTemplateGet(ShareTemplateGetRequest request);

        Task<ResponseBase> ShareTemplateAdd(ShareTemplateAddRequest request);

        Task<ResponseBase> ShareTemplateEdit(ShareTemplateEditRequest request);

        Task<ResponseBase> ShareTemplateDel(ShareTemplateDelRequest request);

        Task<ResponseBase> ShareTemplateChangeStatus(ShareTemplateChangeStatusRequest request);

        Task<ResponseBase> ShareTemplateGetPaging(ShareTemplateGetPagingRequest request);
    }
}
