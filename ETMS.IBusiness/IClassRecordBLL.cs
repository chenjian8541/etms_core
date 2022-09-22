using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassRecordBLL : IBaseBLL
    {
        Task<ResponseBase> ClassRecordGetPaging(ClassRecordGetPagingRequest request);

        Task<ResponseBase> ClassRecordGetPagingSimple(ClassRecordGetPagingRequest request);

        Task<ResponseBase> ClassRecordStudentGetSimple(ClassRecordStudentGetSimpleRequest request);

        Task<ResponseBase> ClassRecordGetPagingH5(ClassRecordGetPagingRequest request);

        Task<ResponseBase> ClassRecordGet(ClassRecordGetRequest request);

        Task<ResponseBase> ClassRecordStudentGet(ClassRecordStudentGetRequest request);

        Task<ResponseBase> ClassRecordOperationLogGetPaging(ClassRecordOperationLogGetPagingRequest request);

        Task<ResponseBase> ClassRecordAbsenceLogPaging(ClassRecordAbsenceLogPagingRequest request);

        Task<ResponseBase> ClassRecordAbsenceLogHandle(ClassRecordAbsenceLogHandleRequest request);

        Task<ResponseBase> ClassRecordPointsApplyHandle(ClassRecordPointsApplyHandleRequest request);

        Task<ResponseBase> ClassRecordPointsApplyHandleBatch(ClassRecordPointsApplyHandleBatchRequest request);

        Task<ResponseBase> StudentClassRecordGetPaging(StudentClassRecordGetPagingRequest request);

        Task<ResponseBase> ClassRecordPointsApplyLogPaging(ClassRecordPointsApplyLogPagingRequest request);

        Task<ResponseBase> ClassRecordStudentChange(ClassRecordStudentChangeRequest request);
    }
}
