using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IGradeBLL : IBaseBLL
    {
        Task<ResponseBase> GradeAdd(GradeAddRequest request);

        Task<ResponseBase> GradeGet(GradeGetRequest request);

        Task<ResponseBase> GradeDel(GradeDelRequest request);
    }
}
