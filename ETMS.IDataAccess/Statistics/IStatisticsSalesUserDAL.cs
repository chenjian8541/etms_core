using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsSalesUserDAL : IBaseDAL
    {
        Task<EtStatisticsSalesUser> GetStatisticsSalesUser(long userId, DateTime ot);

        Task SaveStatisticsSalesUser(EtStatisticsSalesUser entity);

        Task<IEnumerable<StatisticsSalesUserView>> GetStatisticsSalesUser(DateTime startTime, DateTime endTime);
    }
}
