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

        Task<ResponseBase> StudentLeaveApplyDetailGet(StudentLeaveApplyDetailGetRequest request);

        Task<ResponseBase> StudentLeaveApplyRevoke(StudentLeaveApplyRevokeRequest request);

        Task<ResponseBase> StudentClassTimetableGet(StudentClassTimetableRequest request);

        Task<ResponseBase> StudentClassTimetableDetailGet(StudentClassTimetableDetailGetRequest request);

        Task<ResponseBase> IndexBannerGet(IndexBannerGetRequest request);
    }
}
