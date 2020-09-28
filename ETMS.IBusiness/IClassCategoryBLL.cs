using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassCategoryBLL : IBaseBLL
    {
        Task<ResponseBase> ClassCategoryAdd(ClassCategoryAddRequest request);

        Task<ResponseBase> ClassCategoryGet(ClassCategoryGetRequest request);

        Task<ResponseBase> ClassCategoryDel(ClassCategoryDelRequest request);
    }
}
