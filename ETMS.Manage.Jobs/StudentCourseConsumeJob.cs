using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public StudentCourseConsumeJob(ISysTenantDAL sysTenantDAL, IJobAnalyzeBLL analyzeClassTimesBLL, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._analyzeClassTimesBLL = analyzeClassTimesBLL;
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
            var tenantList = await _sysTenantDAL.GetTenants();
            var deTime = DateTime.Now.AddDays(-1).Date;
            foreach (var tenant in tenantList)
            {
                var pageCurrent = 1;
                this._analyzeClassTimesBLL.ResetTenantId(tenant.Id);
                var needConsumeStudentCourse = await _analyzeClassTimesBLL.GetNeedConsumeStudentCourse(_pageSize, pageCurrent, deTime);
                if (needConsumeStudentCourse.Item2 == 0)
                {
                    continue;
                }
                HandleConsumeStudentCourse(tenant.Id, needConsumeStudentCourse.Item1, deTime);
                var totalPage = EtmsHelper.GetTotalPage(needConsumeStudentCourse.Item2, _pageSize);
                pageCurrent++;
                while (pageCurrent <= totalPage)
                {
                    var ruleResult = await _analyzeClassTimesBLL.GetNeedConsumeStudentCourse(_pageSize, pageCurrent, deTime);
                    HandleConsumeStudentCourse(tenant.Id, ruleResult.Item1, deTime);
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
