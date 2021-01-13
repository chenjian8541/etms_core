using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentTransferCourses : IBaseBLL
    {
        Task<ResponseBase> TransferCourses(TransferCoursesRequest request);
    }
}
