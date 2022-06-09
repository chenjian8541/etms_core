using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Pay.Suixing.Utility
{
    internal class Com
    {
        public static string GetReqId()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
