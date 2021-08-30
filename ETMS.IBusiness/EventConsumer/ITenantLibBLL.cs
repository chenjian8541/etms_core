using ETMS.Entity.Database.Source;
using ETMS.Event.DataContract;
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
    }
}
