using ETMS.Entity.Common;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface IClassCheckSignRevokeBLL: IBaseBLL
    {
        Task<ResponseBase> ClassCheckSignRevoke(ClassCheckSignRevokeRequest request);

        Task ClassCheckSignRevokeEvent(ClassCheckSignRevokeEvent request);
    }
}
