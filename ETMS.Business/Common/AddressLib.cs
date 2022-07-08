using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    public class AddressLib
    {
        internal static string GetMicroWebHomeUrl(string baseUrl, int tenantId)
        {
            return string.Format(baseUrl, TenantLib.GetTenantEncrypt(tenantId));
        }
    }
}
