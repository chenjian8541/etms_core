using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class DangerousVisitor
    {
        public string Url { get; set; }

        public string RemoteIpAddress { get; set; }

        public string LocalIpAddress { get; set; }

        public DateTime Time { get; set; }
    }
}
