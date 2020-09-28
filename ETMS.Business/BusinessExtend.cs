using ETMS.IBusiness;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Business
{
    public static class BusinessExtend
    {
        public static void InitDataAccess(this IBaseBLL @this, int tenantId, params IBaseDAL[] dals)
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

        public static void ResetDataAccess(this IBaseBLL @this, int tenantId, params IBaseDAL[] dals)
        {
            if (dals == null || dals.Length == 0)
            {
                return;
            }
            foreach (var dal in dals)
            {
                dal.ResetTenantId(tenantId);
            }
        }
    }
}
