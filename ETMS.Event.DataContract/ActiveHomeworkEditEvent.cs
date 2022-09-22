using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class ActiveHomeworkEditEvent : Event
    {
        public ActiveHomeworkEditEvent(int tenantId) : base(tenantId)
        {
        }

        public EtActiveHomework ActiveHomework { get; set; }
    }
}
