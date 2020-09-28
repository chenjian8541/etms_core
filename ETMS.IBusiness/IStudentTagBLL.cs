using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentTagBLL : IBaseBLL
    {
        Task<ResponseBase> StudentTagAdd(StudentTagAddRequest request);

        Task<ResponseBase> StudentTagGet(StudentTagGetRequest request);

        Task<ResponseBase> StudentTagDel(StudentTagDelRequest request);
    }
}
