using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Manage.Jobs
{
    public class AnalyzeStudentJob : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private readonly IJobAnalyzeBLL _jobAnalyzeBLL;

        private const int _pageSize = 100;

        public AnalyzeStudentJob(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher,
            IJobAnalyzeBLL jobAnalyzeBLL) : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            this._jobAnalyzeBLL = jobAnalyzeBLL;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            var pageCurrent = 1;
            this._jobAnalyzeBLL.ResetTenantId(tenant.Id);
            var myStudentResult = await _jobAnalyzeBLL.GetStudent(_pageSize, pageCurrent);
            if (myStudentResult.Item2 == 0)
            {
                return;
            }
            HandleStudent(tenant.Id, myStudentResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(myStudentResult.Item2, _pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                var studentResult = await _jobAnalyzeBLL.GetStudent(_pageSize, pageCurrent);
                HandleStudent(tenant.Id, studentResult.Item1);
                pageCurrent++;
            }
        }

        private void HandleStudent(int tenantId, IEnumerable<EtStudent> students)
        {
            foreach (var p in students)
            {
                //_eventPublisher.Publish(new SyncStudentClassInfoEvent(tenantId)
                //{
                //    StudentId = p.Id
                //});
                _eventPublisher.Publish(new UpdateStudentInfoEvent(tenantId)
                {
                    MyStudent = p,
                    IsAnalyzeStudentClass = true
                });
            }
        }
    }
}
