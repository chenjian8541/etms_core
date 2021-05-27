using ETMS.Entity.Common;
using ETMS.Entity.EtmsManage.Dto.DataLog.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysSmsTemplateBLL
    {
        Task<ResponseBase> SysSmsTemplateGetPaging(SysSmsTemplateGetPagingRequest request);

        Task<ResponseBase> SysSmsTemplateHandle(SysSmsTemplateHandleRequest request);
    }
}
