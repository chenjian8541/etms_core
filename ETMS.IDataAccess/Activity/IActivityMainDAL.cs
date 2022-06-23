using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess.Activity
{
    public interface IActivityMainDAL : IBaseDAL
    {
        Task AddActivityMain(EtActivityMain entity);

        Task UpdateActivityMainShareQRCode(long id, string shareQRCode);

        Task UpdateActivityMainStatus(long id, int newActivityStatus);

        Task UpdateActivityMainIsShowInParent(long id, bool isShowInParent);

        Task EditActivityMain(EtActivityMain entity);

        Task<EtActivityMain> GetActivityMain(long id);

        Task<Tuple<IEnumerable<EtActivityMain>, int>> GetPaging(IPagingRequest request);

        Task AddBehaviorCount(long activityId, int addPVCount, int addUVCount, int addTranspondCount, int addVisitCount);

        Task SetEffectCount(long activityId, int joinCount, int routeCount, int finishCount);

        Task DelActivityMain(long activityId);
    }
}
