using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Activity
{
    public interface IActivityRoute2DAL : IBaseDAL
    {
        void RemoveCache(long activityRouteId);

        Task<EtActivityRoute> GetActivityRoute(long activityRouteId);
    }
}
