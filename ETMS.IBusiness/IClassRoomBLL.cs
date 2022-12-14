using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassRoomBLL : IBaseBLL
    {
        Task<ResponseBase> ClassRoomAdd(ClassRoomAddRequest request);

        Task<ResponseBase> ClassRoomGet(ClassRoomGetRequest request);

        Task<ResponseBase> ClassRoomDel(ClassRoomDelRequest request);
    }
}
