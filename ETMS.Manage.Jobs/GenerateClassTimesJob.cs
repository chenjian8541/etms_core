using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.LOG;
using ETMS.Manage.Entity.Config;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.Manage.Jobs
{
    /// <summary>
    /// 定时生成课次信息
    /// </summary>
    public class GenerateClassTimesJob : BaseJob
    {
        private readonly ISysTenantDAL _sysTenantDAL;

        private readonly IJobAnalyzeBLL _analyzeClassTimesBLL;

        private readonly IEventPublisher _eventPublisher;

        private const int _pageSize = 100;

        public GenerateClassTimesJob(ISysTenantDAL sysTenantDAL, IJobAnalyzeBLL analyzeClassTimesBLL, IEventPublisher eventPublisher)
        {
            this._sysTenantDAL = sysTenantDAL;
            this._analyzeClassTimesBLL = analyzeClassTimesBLL;
            this._eventPublisher = eventPublisher;
        }

        public override async Task Process(JobExecutionContext context)
        {
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
                await _analyzeClassTimesBLL.UpdateClassTimesRuleLoopStatus();
                var loopClassTimesRuleResult = await _analyzeClassTimesBLL.GetNeedLoopClassTimesRule(_pageSize, pageCurrent);
                if (loopClassTimesRuleResult.Item2 == 0)
                {
                    continue;
                }
                HandleClassTimesRule(tenant.Id, loopClassTimesRuleResult.Item1);
                var totalPage = EtmsHelper.GetTotalPage(loopClassTimesRuleResult.Item2, _pageSize);
                pageCurrent++;
                while (pageCurrent <= totalPage)
                {
                    var ruleResult = await _analyzeClassTimesBLL.GetNeedLoopClassTimesRule(_pageSize, pageCurrent);
                    HandleClassTimesRule(tenant.Id, ruleResult.Item1);
                    pageCurrent++;
                }
            }
        }

        private void HandleClassTimesRule(int tenantId, IEnumerable<LoopClassTimesRule> rules)
        {
            foreach (var r in rules)
            {
                _eventPublisher.Publish(new GenerateClassTimesEvent(tenantId)
                {
                    ClassTimesRuleId = r.Id,
                    ClassId = r.ClassId
                });
            }
        }
    }
}
