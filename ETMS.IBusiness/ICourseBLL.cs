using ETMS.Entity.Common;
using ETMS.Entity.Dto.Product.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ICourseBLL : IBaseBLL
    {
        Task<ResponseBase> CourseAdd(CourseAddRequest request);

        Task<ResponseBase> CourseEdit(CourseEditRequest request);

        Task<ResponseBase> CourseGet(CourseGetRequest request);

        Task<ResponseBase> CourseDel(CourseDelRequest request);

        Task<ResponseBase> CourseChangeStatus(CourseChangeStatusRequest request);

        Task<ResponseBase> CourseGetPaging(CourseGetPagingRequest request);

        Task<ResponseBase> CourseViewGet(CourseViewGetRequest request);
    }
}
