using ETMS.Entity.Common;
using ETMS.Entity.Dto.Interaction.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IActiveWxMessageBLL : IBaseBLL
    {
        Task<ResponseBase> ActiveWxMessageAdd(ActiveWxMessageAddRequest request);

        Task ActiveWxMessageAddConsumerEvent(ActiveWxMessageAddEvent request);

        Task<ResponseBase> ActiveWxMessageEdit(ActiveWxMessageEditRequest request);

        Task<ResponseBase> ActiveWxMessageDel(ActiveWxMessageDelRequest request);

        Task<ResponseBase> ActiveWxMessageGet(ActiveWxMessageGetRequest request);

        Task<ResponseBase> ActiveWxMessageGetPaging(ActiveWxMessageGetPagingRequest request);
    }
}
