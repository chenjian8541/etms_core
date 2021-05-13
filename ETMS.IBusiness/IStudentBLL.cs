using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentBLL : IBaseBLL
    {
        Task<ResponseBase> StudentDuplicateCheck(StudentDuplicateCheckRequest request);

        Task<ResponseBase> StudentAdd(StudentAddRequest request);

        Task<ResponseBase> StudentEdit(StudentEditRequest request);

        Task<ResponseBase> StudentDel(StudentDelRequest request);

        Task<ResponseBase> StudentGet(StudentGetRequest request);

        Task<ResponseBase> StudentGetForEdit(StudentGetForEditReuqest request);

        Task<ResponseBase> StudentGetPaging(StudentGetPagingRequest request);

        Task<ResponseBase> StudentSetTrackUser(StudentSetTrackUserRequest request);

        Task<ResponseBase> StudentSetLearningManager(StudentSetLearningManagerRequest request);

        Task<ResponseBase> StudentTrackLogAdd(StudentTrackLogAddRequest request);

        Task<ResponseBase> StudentTrackLogGetLast(StudentTrackLogGetLastRequest request);

        Task<ResponseBase> StudentTrackLogGetList(StudentTrackLogGetListRequest request);

        Task<ResponseBase> StudentTrackLogDel(StudentTrackLogDelRequest request);

        Task<ResponseBase> StudentTrackLogGetPaging(StudentTrackLogGetPagingRequest request);

        Task<ResponseBase> StudentOperationLogPaging(StudentOperationLogPagingRequest request);

        ResponseBase StudentOperationLogTypeGet(RequestBase request);

        Task<ResponseBase> StudentLeaveApplyLogGet(StudentLeaveApplyLogGetRequest request);

        Task<ResponseBase> StudentLeaveApplyLogPaging(StudentLeaveApplyLogPagingRequest request);

        Task<ResponseBase> StudentLeaveApplyHandle(StudentLeaveApplyHandleRequest request);

        Task<ResponseBase> StudentLeaveApplyAdd(StudentLeaveApplyAddRequest request);

        Task<ResponseBase> StudentExtendFieldInit(StudentExtendFieldInitRequest request);

        Task<ResponseBase> StudentMarkGraduation(StudentMarkGraduationRequest request);

        Task<ResponseBase> StudentMarkReading(StudentMarkReadingRequest request);

        Task<ResponseBase> StudentMarkHidden(StudentMarkHiddenRequest request);

        [Obsolete("已过时，参考StudentLeaveAboutClassCheckSignGet方法")]
        Task<ResponseBase> StudentLeaveApplyPassGet(StudentLeaveApplyPassGetRequest request);

        Task<ResponseBase> StudentLeaveAboutClassCheckSignGet(StudentLeaveAboutClassCheckSignGetRequest request);

        Task<ResponseBase> StudentGetByCardNo(StudentGetByCardNoRequest request);

        Task<ResponseBase> StudentRelieveCardNo(StudentRelieveCardNoRequest request);

        Task<ResponseBase> StudentBindingCardNo(StudentBindingCardNoRequest request);

        Task<ResponseBase> StudentChangePoints(StudentChangePointsRequest request);

        Task<ResponseBase> StudentChangePwd(StudentChangePwdRequest request);
    }
}
