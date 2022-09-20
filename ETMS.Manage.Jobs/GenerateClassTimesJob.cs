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
    public class GenerateClassTimesJob : BaseTenantHandle
    {
        private readonly IJobAnalyzeBLL _analyzeClassTimesBLL;

        private readonly IEventPublisher _eventPublisher;

        private const int _pageSize = 100;

        private DateTime _maxLimitTime;

        public GenerateClassTimesJob(ISysTenantDAL sysTenantDAL, IJobAnalyzeBLL analyzeClassTimesBLL,
            IEventPublisher eventPublisher) : base(sysTenantDAL)
        {
            this._analyzeClassTimesBLL = analyzeClassTimesBLL;
            this._eventPublisher = eventPublisher;
            _maxLimitTime = EtmsHelper3.GetClassTimesMaxDate();
        }

        public override async Task ProcessTenant(SysTenant tenant)
        {
            var pageCurrent = 1;
            this._analyzeClassTimesBLL.ResetTenantId(tenant.Id);
            await _analyzeClassTimesBLL.UpdateClassTimesRuleLoopStatus();
            var loopClassTimesRuleResult = await _analyzeClassTimesBLL.GetNeedLoopClassTimesRule(_pageSize, pageCurrent, _maxLimitTime);
            if (loopClassTimesRuleResult.Item2 == 0)
            {
                return;
            }
            HandleClassTimesRule(tenant.Id, loopClassTimesRuleResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(loopClassTimesRuleResult.Item2, _pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                var ruleResult = await _analyzeClassTimesBLL.GetNeedLoopClassTimesRule(_pageSize, pageCurrent, _maxLimitTime);
                HandleClassTimesRule(tenant.Id, ruleResult.Item1);
                pageCurrent++;
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
