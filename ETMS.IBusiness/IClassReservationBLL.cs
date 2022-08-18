using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassReservationBLL: IBaseBLL
    {
        Task<ResponseBase> ClassReservationRuleGet(RequestBase request);

        Task<ResponseBase> ClassReservationRuleSave(ClassReservationRuleSaveRequest request);

        Task<ResponseBase> ClassReservationLogGetPaging(ClassReservationLogGetPagingRequest request);

        Task<ResponseBase> ReservationCourseSetGet(RequestBase request);

        Task<ResponseBase> ReservationCourseSetAdd(ReservationCourseSetAddRequest request);

        Task<ResponseBase> ReservationCourseSetEdit(ReservationCourseSetEditRequest request);

        Task<ResponseBase> ReservationCourseSetDel(ReservationCourseSetDelRequest request);
    }
}
