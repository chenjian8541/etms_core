using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IParentDataBLL : IBaseBLL
    {
        Task<ResponseBase> StudentLeaveApplyGet(StudentLeaveApplyGetRequest request);

        Task<ResponseBase> StudentListGet(StudentListGetRequest request);

        Task<ResponseBase> StudentListDetailGet(StudentListDetailGetRequest request);

       Task<ResponseBase> StudentLeaveApplyDetailGet(StudentLeaveApplyDetailGetRequest request);

        Task<ResponseBase> StudentLeaveApplyRevoke(StudentLeaveApplyRevokeRequest request);

        Task<ResponseBase> StudentClassTimetableGet(StudentClassTimetableRequest request);

        Task<ResponseBase> StudentClassTimetableDetailGet(StudentClassTimetableDetailGetRequest request);

        Task<ResponseBase> IndexBannerGet(IndexBannerGetRequest request);

        Task<ResponseBase> HomeworkUnansweredGetPaging(HomeworkUnansweredGetPagingRequest request);

        Task<ResponseBase> HomeworkAnsweredGetPaging(HomeworkAnsweredGetPagingRequest request);

        Task<ResponseBase> HomeworkDetailGet(HomeworkDetailGetRequest request);

        Task<ResponseBase> HomeworkDetailSetRead(HomeworkDetailSetReadRequest request);

        Task<ResponseBase> HomeworkSubmitAnswer(HomeworkSubmitAnswerRequest request);

        Task<ResponseBase> HomeworkAddComment(HomeworkAddCommentRequest request);

        Task<ResponseBase> HomeworkDelComment(HomeworkDelCommentRequest request);

        Task<ResponseBase> GrowthRecordGetPaging(GrowthRecordGetPagingRequest request);

        Task<ResponseBase> GrowthRecordFavoriteGetPaging(GrowthRecordGetPagingRequest request);

        Task<ResponseBase> GrowthRecordDetailGet(GrowthRecordDetailGetRequest request);

        Task<ResponseBase> GrowthRecordChangeFavorite(GrowthRecordChangeFavoriteRequest request);

        Task<ResponseBase> GrowthRecordAddComment(GrowthRecordAddCommentRequest request);

        Task<ResponseBase> GrowthRecordDelComment(GrowthRecordDelCommentRequest request);
    }
}
