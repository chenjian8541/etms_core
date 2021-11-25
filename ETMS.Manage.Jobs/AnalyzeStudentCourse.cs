using ETMS.Entity.Database.Manage;
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
    public class AnalyzeStudentCourse : BaseTenantHandle
    {
        private readonly IEventPublisher _eventPublisher;

        private readonly IJobAnalyzeBLL _jobAnalyzeBLL;

        private const int _pageSize = 100;

        public AnalyzeStudentCourse(ISysTenantDAL sysTenantDAL, IEventPublisher eventPublisher,
            IJobAnalyzeBLL jobAnalyzeBLL) : base(sysTenantDAL)
        {
            this._eventPublisher = eventPublisher;
            this._jobAnalyzeBLL = jobAnalyzeBLL;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            var pageCurrent = 1;
            this._jobAnalyzeBLL.ResetTenantId(tenant.Id);
            var hasCourseStudentResult = await _jobAnalyzeBLL.GetHasCourseStudent(_pageSize, pageCurrent);
            if (hasCourseStudentResult.Item2 == 0)
            {
                return;
            }
            HandleStudent(tenant.Id, hasCourseStudentResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(hasCourseStudentResult.Item2, _pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                var studentResult = await _jobAnalyzeBLL.GetHasCourseStudent(_pageSize, pageCurrent);
                HandleStudent(tenant.Id, studentResult.Item1);
                pageCurrent++;
            }
        }

        private void HandleStudent(int tenantId, IEnumerable<HasCourseStudent> students)
        {
            foreach (var p in students)
            {
                _eventPublisher.Publish(new StudentCourseAnalyzeEvent(tenantId)
                {
                    StudentId = p.Id
                });
            }
        }
    }
}
