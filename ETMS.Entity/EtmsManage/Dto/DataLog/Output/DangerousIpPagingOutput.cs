
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.DataLog.Output
{
    public class DangerousIpPagingOutput
    {
        public string Url { get; set; }

        public string RemoteIpAddress { get; set; }

        public string LocalIpAddress { get; set; }

        public DateTime Ot { get; set; }
    }
}
