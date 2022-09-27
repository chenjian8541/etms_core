using ETMS.Entity.Common;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IActiveGrowthRecordBLL : IBaseBLL
    {
        Task<ResponseBase> ActiveGrowthRecordClassGetPaging(ActiveGrowthRecordClassGetPagingRequest request);

        Task<ResponseBase> ActiveGrowthRecordStudentGetPaging(ActiveGrowthRecordStudentGetPagingRequest request);

        Task<ResponseBase> ActiveGrowthRecordStudentStatusGet(ActiveGrowthRecordStudentStatusGetRequest request);

        Task<ResponseBase> ActiveGrowthRecordClassAdd(ActiveGrowthRecordClassAddRequest request);

        Task<ResponseBase> ActiveGrowthRecordStudentAdd(ActiveGrowthRecordStudentAddRequest request);

        Task ActiveGrowthRecordAddConsumerEvent(ActiveGrowthRecordAddEvent request);

        Task ActiveGrowthRecordEditConsumerEvent(ActiveGrowthRecordEditEvent request);

        Task<ResponseBase> ActiveGrowthRecordGet(ActiveGrowthRecordGetRequest request);

        Task<ResponseBase> ActiveGrowthRecordDel(ActiveGrowthRecordDelRequest request);

        Task<ResponseBase> ActiveGrowthCommentAdd(ActiveGrowthCommentAddRequest request);

        Task<ResponseBase> ActiveGrowthCommentDel(ActiveGrowthCommentDelRequest request);

        Task<ResponseBase> ActiveGrowthStudentGetPaging(ActiveGrowthStudentGetPagingRequest request);

        Task<ResponseBase> ActiveGrowthGetForEdit(ActiveGrowthGetForEditRequest request);

        Task<ResponseBase> ActiveGrowthEdit(ActiveGrowthEditRequest request);
    }
}
