using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IDataAccess
{
    public interface IStatisticsSalesTenantDAL : IBaseDAL
    {
        Task<EtStatisticsSalesTenant> GetStatisticsSalesTenant(DateTime ot);

        Task SaveStatisticsSalesTenant(EtStatisticsSalesTenant entity);

        Task<StatisticsSalesTenantView> GetStatisticsSalesTenant(DateTime startTime, DateTime endTime);
    }
}
