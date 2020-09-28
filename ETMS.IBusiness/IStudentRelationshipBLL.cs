using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentRelationshipBLL : IBaseBLL
    {
        Task<ResponseBase> StudentRelationshipAdd(StudentRelationshipAddRequest request);

        Task<ResponseBase> StudentRelationshipGet(StudentRelationshipGetRequest request);

        Task<ResponseBase> StudentRelationshipDel(StudentRelationshipDelRequest request);
    }
}
