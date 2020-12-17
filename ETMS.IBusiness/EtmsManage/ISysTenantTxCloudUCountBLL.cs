using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EtmsManage
{
    public interface ISysTenantTxCloudUCountBLL:IBaseBLL
    {
        Task TenantTxCloudUCountConsumerEvent(TenantTxCloudUCountEvent request);
    }
}
