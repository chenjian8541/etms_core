using ETMS.Entity.Common;
using ETMS.Entity.Dto.Student.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IStudent3BLL : IBaseBLL
    {
        Task<ResponseBase> SendToClassNotice(SendToClassNoticeRequest request);
    }
}
