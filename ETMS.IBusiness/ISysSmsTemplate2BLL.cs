using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ISysSmsTemplate2BLL : IBaseBLL
    {
        Task<string> GetSmsTemplate(int tenantId, int type);

        Task<ResponseBase> SysSmsTemplateGet(SysSmsTemplateGetRequest request);

        Task<ResponseBase> SysSmsTemplateSave(SysSmsTemplateSaveRequest request);

        Task<ResponseBase> SysSmsTemplateDel(SysSmsTemplateResetRequest request);
    }
}
