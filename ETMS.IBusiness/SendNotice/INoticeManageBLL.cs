using ETMS.Event.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness
{
    public interface INoticeManageBLL
    {
        Task NoticeManageConsumerEvent(NoticeManageEvent request);
    }
}
