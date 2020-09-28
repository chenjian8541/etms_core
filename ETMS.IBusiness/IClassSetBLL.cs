using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassSetBLL : IBaseBLL
    {
        Task<ResponseBase> ClassSetAdd(ClassSetAddRequest request);

        Task<ResponseBase> ClassSetGet(ClassSetGetRequest request);

        Task<ResponseBase> ClassSetDel(ClassSetDelRequest request);
    }
}
