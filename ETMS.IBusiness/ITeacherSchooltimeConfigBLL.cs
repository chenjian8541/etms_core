using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational2.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ITeacherSchooltimeConfigBLL: IBaseBLL
    {
        Task<ResponseBase> TeacherSchooltimeConfigGetPaging(TeacherSchooltimeConfigGetPagingRequest request);

        Task<ResponseBase> TeacherSchooltimeConfigGet(TeacherSchooltimeConfigGetRequest request);

        Task<ResponseBase> TeacherSchooltimeConfigAdd(TeacherSchooltimeConfigAddRequest request);

        Task<ResponseBase> TeacherSchooltimeConfigDel(TeacherSchooltimeConfigDelRequest request);

        Task<ResponseBase> TeacherSchooltimeConfigExcludeSave(TeacherSchooltimeConfigExcludeSaveRequest request);

        Task<ResponseBase> TeacherSchooltimeSetBatch(TeacherSchooltimeSetBatchRequest request);
    }
}
