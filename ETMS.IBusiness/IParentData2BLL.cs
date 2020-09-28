using ETMS.Entity.Common;
using ETMS.Entity.Dto.Parent.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IParentData2BLL : IBaseBLL
    {
        Task<ResponseBase> ClassRecordGet(ClassRecordGetRequest request);

        Task<ResponseBase> ClassRecordDetailGet(ClassRecordDetailGetRequest request);

        Task<ResponseBase> StudentCourseGet(StudentCourseGetRequest request);

        Task<ResponseBase> StudentOrderGet(StudentOrderGetRequest request);

        Task<ResponseBase> StudentOrderDetailGet(StudentOrderDetailGetRequest request);

        Task<ResponseBase> StudentPointsLogGet(StudentPointsLogGetRequest request);

        Task<ResponseBase> StudentCouponsGet(StudentCouponsGetRequest request);

        Task<ResponseBase> StudentDetailInfo(StudentDetailInfoRequest request);
    }
}
