using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudentGrowingTagBLL : IBaseBLL
    {
        Task<ResponseBase> StudentGrowingTagAdd(StudentGrowingTagAddRequest request);

        Task<ResponseBase> StudentGrowingTagGet(StudentGrowingTagGetRequest request);

        Task<ResponseBase> StudentGrowingTagDel(StudentGrowingTagDelRequest request);
    }
}
