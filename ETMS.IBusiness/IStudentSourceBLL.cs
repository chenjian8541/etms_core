using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentSourceBLL : IBaseBLL
    {
        Task<ResponseBase> StudentSourceAdd(StudentSourceAddRequest request);

        Task<ResponseBase> StudentSourceGet(StudentSourceGetRequest request);

        Task<ResponseBase> StudentSourceDel(StudentSourceDelRequest request);
    }
}
