using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.ExternalService.ExProtocol.ZhuTong.Request
{
    public class SendSmsRequest
    {
        public string username { get; set; }

        public string password { get; set; }

        public string tKey { get; set; }

        public string mobile { get; set; }

        public string content { get; set; }

        public string time { get; set; }
    }
}
