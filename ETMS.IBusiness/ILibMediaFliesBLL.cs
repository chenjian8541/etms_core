using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ILibMediaFliesBLL : IBaseBLL
    {
        Task<ResponseBase> ImageGetPaging(ImageGetPagingRequest request);

        Task<ResponseBase> ImageAdd(ImageAddRequest request);

        Task<ResponseBase> ImageDel(ImageDelRequest request);

        Task<ResponseBase> ImageListGet(ImageListGetRequest request);

        Task<ResponseBase> AudioGetPaging(AudioGetPagingRequest request);

        Task<ResponseBase> AudioAdd(AudioAddRequest request);

        Task<ResponseBase> AudioDel(AudioDelRequest request);

        Task<ResponseBase> AudioListGet(AudioListGetRequest request);
    }
}
