using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Wx.Request
{
    public class IndexRequest
    {
        public string Signature { get; set; }

        public string Timestamp { get; set; }

        public string Nonce { get; set; }

        public string Echostr { get; set; }
    }
}
