using ETMS.Entity.Database.Manage;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETMS.Manage.Jobs
{
    /// <summary>
    /// 学员课时消耗
    /// </summary>
    public class StudentCourseConsumeJob : BaseTenantHandle
    {
        private readonly IJobAnalyzeBLL _analyzeClassTimesBLL;

        private readonly IEventPublisher _eventPublisher;

        private const int _pageSize = 100;

        private DateTime _deTime;

        public StudentCourseConsumeJob(ISysTenantDAL sysTenantDAL, IJobAnalyzeBLL analyzeClassTimesBLL,
            IEventPublisher eventPublisher) : base(sysTenantDAL)
        {
            this._analyzeClassTimesBLL = analyzeClassTimesBLL;
            this._eventPublisher = eventPublisher;
            _deTime = DateTime.Now.AddDays(-1).Date;
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            var pageCurrent = 1;
            this._analyzeClassTimesBLL.ResetTenantId(tenant.Id);
            var needConsumeStudentCourse = await _analyzeClassTimesBLL.GetNeedConsumeStudentCourse(_pageSize, pageCurrent, _deTime);
            if (needConsumeStudentCourse.Item2 == 0)
            {
                return;
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

        private void HandleConsumeStudentCourse(int tenantId, IEnumerable<StudentCourseConsume> studentCourseConsumes, DateTime deTime)
        {
            foreach (var p in studentCourseConsumes)
            {
                _eventPublisher.Publish(new ConsumeStudentCourseEvent(tenantId, p.Id, deTime));
            }
        }
    }
}
