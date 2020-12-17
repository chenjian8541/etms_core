using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudent2BLL : IBaseBLL
    {
        Task<ResponseBase> StudentFaceListGet(StudentFaceListGetRequest request);

        Task<ResponseBase> StudentRelieveFace(StudentRelieveFaceKeyRequest request);

        Task<ResponseBase> StudentBindingFace(StudentBindingFaceKeyRequest request);
    }
}
