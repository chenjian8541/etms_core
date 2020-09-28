using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentExtendFieldBLL:IBaseBLL
    {
        Task<ResponseBase> StudentExtendFieldAdd(StudentExtendFieldAddRequest request);

        Task<ResponseBase> StudentExtendFieldGet(StudentExtendFieldGetRequest request);

        Task<ResponseBase> StudentExtendFieldDel(StudentExtendFieldDelRequest request);
    }
}
