using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentPointBLL: IBaseBLL
    {
        Task<ResponseBase> StudentPointLogPaging(StudentPointLogPagingRequest request);
    }
}
