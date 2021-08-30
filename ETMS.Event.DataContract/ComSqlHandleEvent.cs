using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class ComSqlHandleEvent : Event
    {
        public ComSqlHandleEvent(int tenantId) : base(tenantId)
        {
        }

        public string Sql { get; set; }
    }
}
