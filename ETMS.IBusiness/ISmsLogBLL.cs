using ETMS.Entity.Common;
using ETMS.Entity.Dto.Marketing.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ISmsLogBLL: IBaseBLL
    {
        Task<ResponseBase> StudentSmsLogGetPaging(StudentSmsLogGetPagingRequest request);
    }
}
