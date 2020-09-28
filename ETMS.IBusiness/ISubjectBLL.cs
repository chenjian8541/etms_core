using ETMS.Entity.Common;
using ETMS.Entity.Dto.BasicData.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface ISubjectBLL : IBaseBLL
    {
        Task<ResponseBase> SubjectAdd(SubjectAddRequest request);

        Task<ResponseBase> SubjectGet(SubjectGetRequest request);

        Task<ResponseBase> SubjectDel(SubjectDelRequest request);
    }
}
