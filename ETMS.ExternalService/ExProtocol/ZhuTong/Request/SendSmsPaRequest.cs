using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.ExternalService.ExProtocol.ZhuTong.Request
{
    public class SendSmsPaRequest
    {
        public string username { get; set; }

        public string password { get; set; }

        public string tKey { get; set; }

        public string time { get; set; }

        public List<SendSmsPaItem> records { get; set; }
    }

    public class SendSmsPaItem
    {
        public string mobile { get; set; }

        public string content { get; set; }
    }
}
