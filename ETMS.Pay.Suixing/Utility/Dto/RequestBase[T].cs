using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto
{
    public class RequestBase<T>
    {
        public string orgId { get; set; }

        public T reqData { get; set; }

        public string reqId { get; set; }

        public string sign { get; set; }

        public string signType { get; set; } = "RSA";

        public string timestamp { get; set; }

        public string version { get; set; } = "1.0";
    }
}
