using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using ETMS.Utility;

namespace ETMS.Manage.Jobs
{
    /// <summary>
    /// 学员课时消耗
    /// </summary>
    public class StudentCourseConsumeJob : BaseJob
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IJobAnalyzeBLL _analyzeClassTimesBLL;

        private readonly IEventPublisher _eventPublisher;

        private const int _pageSize = 100;

        private DateTime _deTime;

        public StudentCourseConsumeJob(ISysTenantDAL sysTenantDAL, IJobAnalyzeBLL analyzeClassTimesBLL, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._analyzeClassTimesBLL = analyzeClassTimesBLL;
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
            _deTime = DateTime.Now.AddDays(-1).Date;

            var pageCurrent = 1;
            var getTenantsEffectiveResult = await _sysTenantDAL.GetTenantsEffective(_pageSize, pageCurrent);
            if (getTenantsEffectiveResult.Item2 == 0)
            {
                return;
            }
            await HandleTenantList(getTenantsEffectiveResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(getTenantsEffectiveResult.Item2, _pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                getTenantsEffectiveResult = await _sysTenantDAL.GetTenantsEffective(_pageSize, pageCurrent);
                await HandleTenantList(getTenantsEffectiveResult.Item1);
                pageCurrent++;
            }
        }

        private async Task HandleTenantList(IEnumerable<SysTenant> tenantList)
        {
            if (tenantList == null || !tenantList.Any())
            {
                return;
            }

            foreach (var tenant in tenantList)
            {
                var pageCurrent = 1;
                this._analyzeClassTimesBLL.ResetTenantId(tenant.Id);
                var needConsumeStudentCourse = await _analyzeClassTimesBLL.GetNeedConsumeStudentCourse(_pageSize, pageCurrent, _deTime);
                if (needConsumeStudentCourse.Item2 == 0)
                {
                    continue;
                }
                HandleConsumeStudentCourse(tenant.Id, needConsumeStudentCourse.Item1, _deTime);
                var totalPage = EtmsHelper.GetTotalPage(needConsumeStudentCourse.Item2, _pageSize);
                pageCurrent++;
                while (pageCurrent <= totalPage)
                {
                    var ruleResult = await _analyzeClassTimesBLL.GetNeedConsumeStudentCourse(_pageSize, pageCurrent, _deTime);
                    HandleConsumeStudentCourse(tenant.Id, ruleResult.Item1, _deTime);
                    pageCurrent++;
                }
            }
        }

        private void HandleConsumeStudentCourse(int tenantId, IEnumerable<StudentCourseConsume> studentCourseConsumes, DateTime deTime)
        {
            foreach (var p in studentCourseConsumes)
            {
                _eventPublisher.Publish(new ConsumeStudentCourseEvent(tenantId, p.Id, deTime));
            }
        }
    }
}
