using ETMS.Entity.Database.Source;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.IBusiness.EventConsumer
{
    public interface ITenantLibBLL : IBaseBLL
    {
        Task<EtNoticeConfig> NoticeConfigGet(int type, byte peopleType, int scenesType);

        Task<IEnumerable<EtClass>> GetStudentInClass(long studentId);

        Task ComSqlHandleConsumerEvent(ComSqlHandleEvent request);

        Task SyncTenantLastOpTimeConsumerEvent(SyncTenantLastOpTimeEvent request);

        Task CloudStorageAnalyzeConsumerEvent(CloudStorageAnalyzeEvent request);

        Task SysTenantStatistics2ConsumerEvent(SysTenantStatistics2Event request);

        Task SysTenantStatisticsWeekAndMonthConsumerEvent(SysTenantStatisticsWeekAndMonthEvent request);
    }
}
