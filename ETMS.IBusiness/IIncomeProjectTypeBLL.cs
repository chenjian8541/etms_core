using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IIncomeProjectTypeBLL: IBaseBLL
    {
        Task<ResponseBase> IncomeProjectTypeAdd(IncomeProjectTypeAddRequest request);

        Task<ResponseBase> IncomeProjectTypeGet(IncomeProjectTypeGetRequest request);

        Task<ResponseBase> IncomeProjectTypeDel(IncomeProjectTypeDelRequest request);
    }
}
