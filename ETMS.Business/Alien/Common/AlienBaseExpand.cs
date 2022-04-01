using ETMS.IBusiness.Alien;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    internal static class AlienBaseExpand
    {
        public static void InitTenantDataAccess(this IAlienBaseBLL @this, int tenantId, params IBaseDAL[] dals)
        {
            if (dals == null || dals.Length == 0)
            {
                return;
            }
            foreach (var dal in dals)
            {
                dal.InitTenantId(tenantId);
            }
        }
    }
}
