using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentAccountRechargeBLL: IBaseBLL
    {
        Task<ResponseBase> StudentAccountRechargeRuleGet(StudentAccountRechargeRuleGetRequest request);

        Task<ResponseBase> StudentAccountRechargeRuleSave(StudentAccountRechargeRuleSaveRequest request);

        Task<ResponseBase> StatisticsStudentAccountRechargeGet(StatisticsStudentAccountRechargeGetRequest request);

        Task<ResponseBase> StudentAccountRechargeLogGetPaging(StudentAccountRechargeLogGetPagingRequest request);

        Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeGetRequest request);

        Task<ResponseBase> StudentAccountRechargeGetPaging(StudentAccountRechargeGetPagingRequest request);

        Task<ResponseBase> StudentAccountRechargeGetByPhone(StudentAccountRechargeGetByPhoneRequest request);

        Task<ResponseBase> StudentAccountRechargeGetByStudentId(StudentAccountRechargeGetByStudentIdRequest request);

        Task<ResponseBase> StudentAccountRechargeCreate(StudentAccountRechargeCreateRequest request);

        Task<ResponseBase> StudentAccountRechargeEditPhone(StudentAccountRechargeEditPhoneRequest request);

        Task<ResponseBase> StudentAccountRecharge(StudentAccountRechargeRequest request);

        Task StudentAccountRechargeConsumerEvent(StudentAccountRechargeEvent request);

        Task<ResponseBase> StudentAccountRefund(StudentAccountRefundRequest request);

        Task StudentAccountRefundConsumerEvent(StudentAccountRefundEvent eventRequest);

        Task<ResponseBase> StudentAccountRechargeGetDetail(StudentAccountRechargeGetDetailRequest request);

        Task<ResponseBase> StudentAccountRechargeBinderAdd(StudentAccountRechargeBinderAddRequest request);

        Task<ResponseBase> StudentAccountRechargeBinderRemove(StudentAccountRechargeBinderRemoveRequest request);
    }
}
