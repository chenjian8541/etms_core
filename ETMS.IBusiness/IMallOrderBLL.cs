using ETMS.Entity.Common;
using ETMS.Entity.Dto.HisData2.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IMallOrderBLL : IBaseBLL
    {
        Task<ResponseBase> MallOrderGetPaging(MallOrderGetPagingRequest request);

        Task<ResponseBase> MallOrderGet(MallOrderGetRequest request);
    }
}
