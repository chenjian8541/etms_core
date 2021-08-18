using ETMS.Entity.Common;
using ETMS.Entity.Dto.Teacher.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ITeacherSalaryBLL : IBaseBLL
    {
        Task<ResponseBase> TeacherSalaryFundsItemsGet(TeacherSalaryFundsItemsGetRequest request);

        Task<ResponseBase> TeacherSalaryFundsItemsAdd(TeacherSalaryFundsItemsAddRequest request);

        Task<ResponseBase> TeacherSalaryFundsItemsDel(TeacherSalaryFundsItemsDelRequest request);

        Task<ResponseBase> TeacherSalaryFundsItemsChangeStatus(TeacherSalaryFundsItemsChangeStatusRequest request);

        Task<ResponseBase> TeacherSalaryClassDayGetPaging(TeacherSalaryClassDayGetPagingRequest request);

        Task<ResponseBase> TeacherSalaryGlobalRuleGet(TeacherSalaryGlobalRuleGetRequest request);

        Task<ResponseBase> TeacherSalaryPerformanceRuleSave(TeacherSalaryPerformanceRuleSaveRequest request);

        Task<ResponseBase> TeacherSalaryIncludeArrivedRuleSave(TeacherSalaryIncludeArrivedRuleSaveRequest request);

        Task<ResponseBase> TeacherSalaryContractGetPaging(TeacherSalaryContractGetPagingRequest request);

        Task<ResponseBase> TeacherSalaryContractGetDetail(TeacherSalaryContractGetDetailRequest request);

        Task<ResponseBase> TeacherSalaryContractChangeComputeType(TeacherSalaryContractChangeComputeTypeRequest request);

        Task<ResponseBase> TeacherSalaryContractSave(TeacherSalaryContractSaveRequest request);

        Task<ResponseBase> TeacherSalaryPayrollGoSettlement(TeacherSalaryPayrollGoSettlementRequest request);

        Task<ResponseBase> TeacherSalaryPayrollGetPaging(TeacherSalaryPayrollGetPagingRequest request);

        Task<ResponseBase> TeacherSalaryPayrollGet(TeacherSalaryPayrollGetRequest request);

        Task<ResponseBase> TeacherSalaryPayrollSetOK(TeacherSalaryPayrollSetOKRequest request);

        Task<ResponseBase> TeacherSalaryPayrollDel(TeacherSalaryPayrollDelRequest request);

        Task<ResponseBase> TeacherSalaryPayrollRepeal(TeacherSalaryPayrollRepealRequest request);
    }
}
