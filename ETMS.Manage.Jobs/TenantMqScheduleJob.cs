using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Event.DataContract;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace ETMS.Manage.Jobs
{
    public class TenantMqScheduleJob : BaseJob
    {
        private readonly IEventPublisher _eventPublisher;

        private const int _pageSize = 100;

        private readonly ISysTenantMqScheduleDAL _sysTenantMqScheduleDAL;

        public TenantMqScheduleJob(IEventPublisher eventPublisher, ISysTenantMqScheduleDAL sysTenantMqScheduleDAL)
        {
            this._eventPublisher = eventPublisher;
            this._sysTenantMqScheduleDAL = sysTenantMqScheduleDAL;
        }

        public override async Task Process(JobExecutionContext context)
        {
            var pageCurrent = 1;
            var maxExTime = DateTime.Now;
            var myMsgResult = await _sysTenantMqScheduleDAL.GetTenantMqSchedule(_pageSize, pageCurrent, maxExTime);
            if (myMsgResult.Item2 == 0)
            {
                return;
            }
            HandleMsg(myMsgResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(myMsgResult.Item2, _pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                var msgResult = await _sysTenantMqScheduleDAL.GetTenantMqSchedule(_pageSize, pageCurrent, maxExTime);
                HandleMsg(msgResult.Item1);
                pageCurrent++;
            }
            await _sysTenantMqScheduleDAL.ClearSysTenantMqSchedule(maxExTime);
        }

        private void HandleMsg(IEnumerable<SysTenantMqSchedule> msgList)
        {
            foreach (var p in msgList)
            {
                if (p.Type == EmSysTenantMqScheduleType.SyncStudentLogOfSurplusCourse)
                {
                    var myEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<SyncStudentLogOfSurplusCourseEvent>(p.SendContent);
                    _eventPublisher.Publish(myEvent);
                }
            }
        }
    }
}
