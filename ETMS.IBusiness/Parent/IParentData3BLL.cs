using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Request;
using ETMS.Entity.Dto.Parent2.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IParentData3BLL : IBaseBLL
    {
        Task<ResponseBase> WxMessageDetailPaging(WxMessageDetailPagingRequest request);

        Task<ResponseBase> WxMessageDetailGet(WxMessageDetailGetRequest request);

        Task<ResponseBase> WxMessageDetailSetRead(WxMessageDetailSetReadRequest request);

        Task<ResponseBase> WxMessageDetailSetConfirm(WxMessageDetailSetConfirmRequest request);

        Task<ResponseBase> WxMessageGetUnreadCount(WxMessageGetUnreadCountRequest request);

        Task<ResponseBase> TryCalssApply(TryCalssApplyRequest request);

        Task<ResponseBase> CheckOnLogGet(CheckOnLogGetRequest request);

        Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeGetRequest request);

        Task<ResponseBase> StudentAccountRechargeRuleGet(StudentAccountRechargeRuleGetRequest request);

        Task<ResponseBase> StudentAccountRechargeLogGetPaging(StudentAccountRechargeLogGetPagingRequest request);

        Task<ResponseBase> TeacherGetPaging(TeacherGetPagingRequest request);

        Task<ResponseBase> StudentReservationTimetable(StudentReservationTimetableRequest request);

        Task<ResponseBase> StudentReservationTimetableDetail(StudentReservationTimetableDetailRequest request);

        Task<ResponseBase> StudentReservationDetail(StudentReservationDetailRequest request);

        Task<ResponseBase> StudentReservationLogGetPaging(StudentReservationLogGetPagingRequest request);

        Task<ResponseBase> StudentReservationLogDetail(StudentReservationLogDetailRequest request);

       Task<ResponseBase> StudentReservationSubmit(StudentReservationSubmitRequest request);

        Task<ResponseBase> StudentReservationCancel(StudentReservationCancelRequest request);
    }
}
