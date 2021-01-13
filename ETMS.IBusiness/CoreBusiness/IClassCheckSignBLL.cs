using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassCheckSignBLL : IBaseBLL
    {
        Task<ResponseBase> ClassCheckSign(ClassCheckSignRequest request);

        Task ClassCheckSignEventProcessEvent(ClassCheckSignEvent request);
    }
}
