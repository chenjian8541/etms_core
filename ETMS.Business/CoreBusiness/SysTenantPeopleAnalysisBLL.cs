using ETMS.Entity.Database.Source;
using ETMS.Entity.Temp.Request;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.Business
{
    public class SysTenantPeopleAnalysisBLL : ISysTenantPeopleAnalysisBLL
    {
        private readonly IJobAnalyze2DAL _jobAnalyze2DAL;

        private readonly ISysTenantUserDAL _sysTenantUserDAL;

        private readonly ISysTenantStudentDAL _sysTenantStudentDAL;

        private readonly IEventPublisher _eventPublisher;

        private const int _pageSize = 100;

        public SysTenantPeopleAnalysisBLL(IJobAnalyze2DAL jobAnalyze2DAL, ISysTenantUserDAL sysTenantUserDAL,
            ISysTenantStudentDAL sysTenantStudentDAL, IEventPublisher eventPublisher)
        {
            this._jobAnalyze2DAL = jobAnalyze2DAL;
            this._sysTenantUserDAL = sysTenantUserDAL;
            this._sysTenantStudentDAL = sysTenantStudentDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _jobAnalyze2DAL);
        }

        public async Task SysTenantUserAnalysisConsumerEvent(SysTenantUserAnalysisEvent request)
        {
            await _sysTenantUserDAL.ResetTenantAllUser(request.TenantId);
            var userRequest = new TenantAnalysisUserGetPagingRequest();
            userRequest.PageSize = _pageSize;
            userRequest.PageCurrent = 1;
            var userPagingData = await _jobAnalyze2DAL.GetUserPaging(userRequest);
            if (userPagingData.Item2 == 0)
            {
                LOG.Log.Warn($"[SysTenantUserAnalysisConsumerEvent]未找到机构任何学员,TenantId:{request.TenantId}", this.GetType());
                return;
            }
            HandleTenantUserAnalysis(userPagingData.Item1, request.TenantId);
            var totalPage = EtmsHelper.GetTotalPage(userPagingData.Item2, _pageSize);
            userRequest.PageCurrent++;
            while (userRequest.PageCurrent <= totalPage)
            {
                var ruleResult = await _jobAnalyze2DAL.GetUserPaging(userRequest);
                HandleTenantUserAnalysis(ruleResult.Item1, request.TenantId);
                userRequest.PageCurrent++;
            }
        }

        private void HandleTenantUserAnalysis(IEnumerable<EtUser> users, int tenantId)
        {
            if (users != null && users.Any())
            {
                foreach (var p in users)
                {
                    _eventPublisher.Publish(new SysTenantUserAnalysisAddEvent(tenantId)
                    {
                        AddUserId = p.Id,
                        AddPhone = p.Phone,
                        IsRefreshCache = false
                    });
                }
            }
            else
            {
                LOG.Log.Error("[HandleTenantUserAnalysis]分页数据计算错误", this.GetType());
            }
        }

        public async Task SysTenantUserAnalysisAddConsumerEvent(SysTenantUserAnalysisAddEvent request)
        {
            await _sysTenantUserDAL.AddTenantUser(request.TenantId, request.AddUserId, request.AddPhone, request.IsRefreshCache);
        }

        /// <summary>
        /// 移除用户 先判断下手机号码是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SysTenantUserAnalysisRemoveConsumerEvent(SysTenantUserAnalysisRemoveEvent request) { }

        public async Task SysTenantStudentAnalysisConsumerEvent(SysTenantStudentAnalysisEvent request)
        {
            
        }

        public async Task SysTenantStudentAnalysisAddConsumerEvent(SysTenantStudentAnalysisAddEvent request) { }

        /// <summary>
        /// 移除学员，需要注意 手机号码是否绑定了其它学员
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SysTenantStudentAnalysisRemoveConsumerEvent(SysTenantStudentAnalysisRemoveEvent request) { }
    }
}
