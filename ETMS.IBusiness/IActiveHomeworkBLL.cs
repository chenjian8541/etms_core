using ETMS.Entity.Common;
using ETMS.Entity.Dto.Interaction.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IActiveHomeworkBLL : IBaseBLL
    {
        Task<ResponseBase> ActiveHomeworkGetPaging(ActiveHomeworkGetPagingRequest request);

        Task<ResponseBase> ActiveHomeworkAdd(ActiveHomeworkAddRequest request);

        Task<ResponseBase> ActiveHomeworkAdd2(ActiveHomeworkAdd2Request request);

        Task<ResponseBase> ActiveHomeworkGetBasc(ActiveHomeworkGetBascRequest request);

        Task<ResponseBase> ActiveHomeworkStudentGetAnswered(ActiveHomeworkGetAnsweredRequest request);

        Task<ResponseBase> ActiveHomeworkStudentGetUnanswered(ActiveHomeworkGetUnansweredRequest request);

        Task<ResponseBase> ActiveHomeworkDel(ActiveHomeworkDelRequest request);

        Task<ResponseBase> ActiveHomeworkCommentAdd(ActiveHomeworkCommentAddRequest request);

        Task<ResponseBase> ActiveHomeworkEdit(ActiveHomeworkEditRequest request);

        Task<ResponseBase> ActiveHomeworkCommentDel(ActiveHomeworkCommentDelRequest request);

        Task<ResponseBase> ActiveHomeworkStudentGetPaging(ActiveHomeworkStudentGetPagingRequest request);

        Task<ResponseBase> ActiveHomeworkStudentGet(ActiveHomeworkStudentGetRequest request);
    }
}
