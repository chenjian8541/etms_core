using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
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

        Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeRequest request);

        Task<ResponseBase> StudentAccountRechargeGetPaging(StudentAccountRechargeGetPagingRequest request);
    }
}
