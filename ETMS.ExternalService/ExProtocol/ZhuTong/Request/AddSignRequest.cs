using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.ExternalService.ExProtocol.ZhuTong.Request
{
    public class AddSignRequest
    {
        public string username { get; set; }

        public string password { get; set; }

        public string tKey { get; set; }

        public string sign { get; set; }

        public string remark { get; set; }
    }
}
