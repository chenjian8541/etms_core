using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Temp.Request;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.LOG;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class StudentAccountRechargeChangeBLL : IStudentAccountRechargeChangeBLL
    {
        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private IDistributedLockDAL _distributedLockDAL;

        private readonly IEventPublisher _eventPublisher;

        public StudentAccountRechargeChangeBLL(IStudentAccountRechargeDAL studentAccountRechargeDAL, IDistributedLockDAL distributedLockDAL,
            IEventPublisher eventPublisher)
        {
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._distributedLockDAL = distributedLockDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentAccountRechargeDAL);
        }

        public async Task StudentAccountRechargeChange(StudentAccountRechargeChangeEvent request)
        {
            request.TryCount++;
            var _lockKey = new StudentAccountRechargeChangeToken(request.TenantId, request.StudentAccountRechargeId);
            if (_distributedLockDAL.LockTake(_lockKey))
            {
                try
                {
                    var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.StudentAccountRechargeId);
                    if (accountLog == null)
                    {
                        LOG.Log.Error("[StudentAccountRechargeChange]充值账户未找到", request, this.GetType());
                    }
                    accountLog.BalanceReal += request.AddBalanceReal;
                    accountLog.BalanceGive += request.AddBalanceGive;
                    accountLog.RechargeSum += request.AddRechargeSum;
                    accountLog.RechargeGiveSum += request.AddRechargeGiveSum;
                    accountLog.BalanceSum = accountLog.BalanceReal + accountLog.BalanceGive;
                    await _studentAccountRechargeDAL.EditStudentAccountRecharge(accountLog);

                    _eventPublisher.Publish(new StatisticsStudentAccountRechargeEvent(request.TenantId));
                }
                finally
                {
                    _distributedLockDAL.LockRelease(_lockKey);
                }
            }
            else
            {
                if (request.TryCount > 6)
                {
                    Log.Error($"【StudentAccountRechargeChange】修改学员账户信息,尝试了6次仍未获得执行锁,参数:{JsonConvert.SerializeObject(request)}", this.GetType());
                    return;
                }
                else
                {
                    _eventPublisher.Publish(request);
                }
            }
        }
    }
}
