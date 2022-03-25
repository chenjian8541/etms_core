using ETMS.IBusiness.Alien;
using ETMS.IDataAccess.Alien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Alien
{
    public static class BusinessAlienExtend
    {
        public static void InitDataAccess(this IAlienBaseBLL @this, int headId, params IBaseAlienDAL[] dals)
        {
            if (dals == null || dals.Length == 0)
            {
                return;
            }
            foreach (var dal in dals)
            {
                dal.InitHeadId(headId);
            }
        }

        public static void ResetDataAccess(this IAlienBaseBLL @this, int headId, params IBaseAlienDAL[] dals)
        {
            if (dals == null || dals.Length == 0)
            {
                return;
            }
            foreach (var dal in dals)
            {
                dal.ResetHeadId(headId);
            }
        }
    }
}
