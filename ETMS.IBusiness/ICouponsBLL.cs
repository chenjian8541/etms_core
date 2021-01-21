using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ICouponsBLL : IBaseBLL
    {
        Task<ResponseBase> CouponsAdd(CouponsAddRequest request);

        Task<ResponseBase> CouponsStatusChange(CouponsStatusChangeRequest request);

        Task<ResponseBase> CouponsDel(CouponsDelRequest request);

        Task<ResponseBase> CouponsGetPaging(CouponsGetPagingRequest request);

        Task<ResponseBase> ParentCouponsReceive(ParentCouponsReceiveRequest request);

        Task ParentCouponsReceiveEvent(ParentCouponsReceiveEvent request);

        Task<ResponseBase> CouponsStudentGetPaging(CouponsStudentGetPagingRequest request);

        Task<ResponseBase> CouponsStudentWriteOff(CouponsStudentWriteOffRequest request);

        Task<ResponseBase> CouponsStudentUsePaging(CouponsStudentUsrPagingRequest request);

         Task<ResponseBase> CouponsStudentGetCanUse(CouponsStudentGetCanUseRequest request);

        Task<ResponseBase> CouponsStudentSend(CouponsStudentSendRequest request);

        Task<ResponseBase> StudentCouponsNormalGet2(StudentCouponsNormalGet2Request request);

        Task<ResponseBase> StudentCouponsUsedGet2(StudentCouponsUsedGet2Request request);

        Task<ResponseBase> StudentCouponsExpiredGet2(StudentCouponsExpiredGet2Request request);
    }
}
