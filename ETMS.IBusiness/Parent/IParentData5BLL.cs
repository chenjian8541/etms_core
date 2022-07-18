using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent5.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.Parent
{
    public interface IParentData5BLL: IBaseBLL
    {
        Task<ResponseBase> StudentReservation1v1Check(StudentReservation1v1CheckRequest request);

        Task<ResponseBase> StudentReservation1v1Init(StudentReservation1v1InitRequest request);

        Task<ResponseBase> StudentReservation1v1DateCheck(StudentReservation1v1DateCheckRequest request);

        Task<ResponseBase> StudentReservation1v1LessonsGet(StudentReservation1v1LessonsGetRequest request);

        Task<ResponseBase> StudentReservation1v1LessonsSubmit(StudentReservation1v1LessonsSubmitRequest request);

        Task<ResponseBase> StudentTryClassGetPaging(StudentTryClassGetPagingRequest request);

        Task<ResponseBase> StudentTryClassGet(StudentTryClassGetRequest request);

        Task<ResponseBase> StudentTryClassSubmit(StudentTryClassSubmitRequest request);

        Task<ResponseBase> StudentTryClassCancel(StudentTryClassCancelRequest request);

        Task<ResponseBase> ActivityMainGetPaging(ActivityMainGetPagingRequest request);

        Task<ResponseBase> ActivityMyGetPaging(ActivityMyGetPagingRequest request);
    }
}
