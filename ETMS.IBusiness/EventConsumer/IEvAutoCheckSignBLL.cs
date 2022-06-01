using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EventConsumer
{
    public interface IEvAutoCheckSignBLL : IBaseBLL
    {
        Task AutoCheckSignTenantConsumerEvent(AutoCheckSignTenantEvent request);

        Task AutoCheckSignClassTimesConsumerEvent(AutoCheckSignClassTimesEvent request);
    }
}
