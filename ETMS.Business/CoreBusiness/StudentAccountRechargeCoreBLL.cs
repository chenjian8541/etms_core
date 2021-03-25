using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Database.Source;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using ETMS.IEventProvider;
using ETMS.LOG;
using ETMS.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class StudentAccountRechargeCoreBLL : IStudentAccountRechargeCoreBLL
    {
        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentDAL _studentDAL;

        private IDistributedLockDAL _distributedLockDAL;

        private readonly IEventPublisher _eventPublisher;

        public StudentAccountRechargeCoreBLL(IStudentAccountRechargeDAL studentAccountRechargeDAL, IStudentDAL studentDAL,
            IDistributedLockDAL distributedLockDAL, IEventPublisher eventPublisher)
        {
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentDAL = studentDAL;
            this._distributedLockDAL = distributedLockDAL;
            this._eventPublisher = eventPublisher;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _studentAccountRechargeDAL, _studentDAL);
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

                    _eventPublisher.Publish(new NoticeStudentAccountRechargeChangedEvent(request.TenantId)
                    {
                        StudentAccountRecharge = accountLog,
                        AddBalanceReal = request.AddBalanceReal,
                        AddBalanceGive = request.AddBalanceGive,
                        OtTime = DateTime.Now
                    });
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

        public async Task<StudentAccountRechargeView> GetStudentAccountRechargeByPhone(string phone)
        {
            var studentAccountRechargeBucket = await _studentAccountRechargeDAL.GetStudentAccountRecharge(phone);
            if (studentAccountRechargeBucket == null || studentAccountRechargeBucket.StudentAccountRecharge == null)
            {
                return null;
            }
            var output = new StudentAccountRechargeView()
            {
                StudentAccountRecharge = studentAccountRechargeBucket.StudentAccountRecharge,
                Binders = new List<StudentAccountRechargeBinderView>()
            };
            if (studentAccountRechargeBucket.StudentAccountRechargeBinders != null &&
                studentAccountRechargeBucket.StudentAccountRechargeBinders.Count > 0)
            {
                foreach (var p in studentAccountRechargeBucket.StudentAccountRechargeBinders)
                {
                    var studentBucket = await _studentDAL.GetStudent(p.StudentId);
                    if (studentBucket == null || studentBucket.Student == null)
                    {
                        continue;
                    }
                    output.Binders.Add(new StudentAccountRechargeBinderView()
                    {
                        Gender = studentBucket.Student.Gender,
                        StudentId = studentBucket.Student.Id,
                        StudentAvatar = studentBucket.Student.Avatar,
                        StudentAvatarUrl = AliyunOssUtil.GetAccessUrlHttps(studentBucket.Student.Avatar),
                        StudentName = studentBucket.Student.Name,
                        StudentPhone = studentBucket.Student.Phone,
                        StudentAccountRechargeBinderId = p.Id
                    });
                }
            }

            return output;
        }

        public async Task<EtStudentAccountRecharge> GetStudentAccountRechargeByPhone2(string phone)
        {
            var studentAccountRechargeBucket = await _studentAccountRechargeDAL.GetStudentAccountRecharge(phone);
            if (studentAccountRechargeBucket == null || studentAccountRechargeBucket.StudentAccountRecharge == null)
            {
                return null;
            }
            return studentAccountRechargeBucket.StudentAccountRecharge;
        }

        public async Task<StudentAccountRechargeView> GetStudentAccountRechargeByStudentId(long studentId)
        {
            var studentAccountRechargeBinder = await _studentAccountRechargeDAL.GetAccountRechargeBinderByStudentId(studentId);
            if (studentAccountRechargeBinder == null)
            {
                return null;
            }
            var studentAccountRecharge = await _studentAccountRechargeDAL.GetStudentAccountRecharge(studentAccountRechargeBinder.StudentAccountRechargeId);
            if (studentAccountRecharge == null)
            {
                return null;
            }
            return await GetStudentAccountRechargeByPhone(studentAccountRecharge.Phone);
        }

        public async Task<EtStudentAccountRecharge> GetStudentAccountRechargeByStudentId2(long studentId)
        {
            var studentAccountRechargeBinder = await _studentAccountRechargeDAL.GetAccountRechargeBinderByStudentId(studentId);
            if (studentAccountRechargeBinder == null)
            {
                return null;
            }
            var studentAccountRecharge = await _studentAccountRechargeDAL.GetStudentAccountRecharge(studentAccountRechargeBinder.StudentAccountRechargeId);
            if (studentAccountRecharge == null)
            {
                return null;
            }
            return await GetStudentAccountRechargeByPhone2(studentAccountRecharge.Phone);
        }
    }
}
