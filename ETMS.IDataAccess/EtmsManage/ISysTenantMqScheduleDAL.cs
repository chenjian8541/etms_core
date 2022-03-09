using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Database.Manage;
using ETMS.Event;

namespace ETMS.IDataAccess.EtmsManage
{
    public interface ISysTenantMqScheduleDAL
    {
        Task AddSysTenantMqSchedule<T>(T entity, int tenantId, int type, TimeSpan timeSpan) where T : Event.DataContract.Event;

        Task<Tuple<IEnumerable<SysTenantMqSchedule>, int>> GetTenantMqSchedule(int pageSize, int pageCurrent, DateTime maxExTime);

        Task ClearSysTenantMqSchedule(DateTime maxExTime);
    }
}
