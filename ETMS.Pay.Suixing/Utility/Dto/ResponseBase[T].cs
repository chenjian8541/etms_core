using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility.Dto
{
    public class ResponseBase<T>
    {
        /// <summary>
        /// <see cref="EmResponseCode"/>
        /// </summary>
        public string code { get; set; }

        public string msg { get; set; }

        public string orgId { get; set; }

        public string reqId { get; set; }

        public T respData { get; set; }

        public string signType { get; set; }

        public string sign { get; set; }
    }
}
