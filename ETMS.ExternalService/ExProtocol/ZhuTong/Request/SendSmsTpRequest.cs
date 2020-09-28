using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.ExternalService.ExProtocol.ZhuTong.Request
{
    public class SendSmsTpRequest<T>
    {
        public string username { get; set; }

        public string password { get; set; }

        public string tKey { get; set; }

        public string signature { get; set; }

        public long tpId { get; set; }

        public List<Records<T>> records { get; set; }
    }

    public class Records<T>
    {
        public string mobile { get; set; }

        public T tpContent { get; set; }
    }
}
