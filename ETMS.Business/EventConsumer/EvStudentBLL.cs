using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.IBusiness;
using ETMS.IBusiness.EventConsumer;
using ETMS.IDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.EventConsumer
{
    public class EvStudentBLL : IEvStudentBLL
    {
        private readonly IStudentDAL _studentDAL;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IAppConfigDAL _appConfigDAL;

        private readonly ITenantConfigDAL _tenantConfigDAL;

        private readonly IStudentAccountRechargeChangeBLL _studentAccountRechargeChangeBLL;

        public EvStudentBLL(IStudentDAL studentDAL, IStudentPointsLogDAL studentPointsLogDAL, IStudentAccountRechargeDAL studentAccountRechargeDAL,
            IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL, IAppConfigDAL appConfigDAL, ITenantConfigDAL tenantConfigDAL,
            IStudentAccountRechargeChangeBLL studentAccountRechargeChangeBLL)
        {
            this._studentDAL = studentDAL;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._appConfigDAL = appConfigDAL;
            this._tenantConfigDAL = tenantConfigDAL;
            this._studentAccountRechargeChangeBLL = studentAccountRechargeChangeBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeChangeBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _studentDAL, _studentPointsLogDAL, _studentAccountRechargeDAL, _studentAccountRechargeLogDAL,
               _appConfigDAL, _tenantConfigDAL);
        }

        public async Task StudentRecommendRewardConsumerEvent(StudentRecommendRewardEvent request)
        {
            if (request.Student.RecommendStudentId == null)
            {
                return;
            }
            var config = await _tenantConfigDAL.GetTenantConfig();
            var studentRecommendConfig = config.StudentRecommendConfig;
            if (request.Type == StudentRecommendRewardType.Registered)
            {
                await StudentRecommendRewardConsumerRegistered(request, studentRecommendConfig);
                return;
            }
            if (request.Type == StudentRecommendRewardType.Buy)
            {
                await StudentRecommendRewardConsumerBuy(request, studentRecommendConfig);
                return;
            }
        }

        private async Task StudentRecommendRewardConsumerRegistered(StudentRecommendRewardEvent request, StudentRecommendConfig studentRecommendConfig)
        {
            if (!studentRecommendConfig.IsOpenRegistered || (studentRecommendConfig.RegisteredGivePoints <= 0 && studentRecommendConfig.RegisteredGiveMoney <= 0))
            {
                return;
            }
            var recommendStudentBucket = await _studentDAL.GetStudent(request.Student.RecommendStudentId.Value);
            if (recommendStudentBucket == null || recommendStudentBucket.Student == null)
            {
                LOG.Log.Error("[StudentRecommendRewardConsumerRegistered]推荐的学员未找到", request, this.GetType());
                return;
            }
            var recommendStudent = recommendStudentBucket.Student;
            var now = DateTime.Now;
            if (studentRecommendConfig.RegisteredGivePoints > 0)
            {
                //奖励积分
                await _studentDAL.AddPoint(recommendStudent.Id, studentRecommendConfig.RegisteredGivePoints);
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = string.Empty,
                    Ot = now,
                    Points = studentRecommendConfig.RegisteredGivePoints,
                    Remark = $"推荐学员注册[{request.Student.Name},{request.Student.Phone}]奖励{studentRecommendConfig.RegisteredGivePoints}积分",
                    StudentId = recommendStudent.Id,
                    TenantId = recommendStudent.TenantId,
                    Type = EmStudentPointsLogType.RecommendStudentRegistered
                });
            }
            if (studentRecommendConfig.RegisteredGiveMoney > 0)
            {
                //奖励金额
                var accountLog = await _studentAccountRechargeChangeBLL.GetStudentAccountRecharge(recommendStudent.Phone, recommendStudent.PhoneBak);
                if (accountLog == null)
                {
                    return;
                }
                await _studentAccountRechargeChangeBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(recommendStudent.TenantId)
                {
                    AddBalanceReal = 0,
                    AddBalanceGive = studentRecommendConfig.RegisteredGiveMoney,
                    AddRechargeSum = 0,
                    AddRechargeGiveSum = 0,
                    StudentAccountRechargeId = accountLog.Id,
                    TryCount = 0
                });
                await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
                {
                    CgNo = string.Empty,
                    CgBalanceReal = 0,
                    CgBalanceGive = studentRecommendConfig.RegisteredGiveMoney,
                    CgServiceCharge = 0,
                    CommissionUser = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Phone = accountLog.Phone,
                    RelatedOrderId = null,
                    Remark = $"推荐学员注册[{request.Student.Name},{request.Student.Phone}]奖励{studentRecommendConfig.RegisteredGiveMoney}金额",
                    Status = EmStudentAccountRechargeLogStatus.Normal,
                    StudentAccountRechargeId = accountLog.Id,
                    TenantId = accountLog.TenantId,
                    UserId = request.UserId,
                    Type = EmStudentAccountRechargeLogType.RecommendStudentRegistered
                });
            }
        }

        private async Task StudentRecommendRewardConsumerBuy(StudentRecommendRewardEvent request, StudentRecommendConfig studentRecommendConfig)
        {
            if (!studentRecommendConfig.IsOpenBuy || (studentRecommendConfig.BuyGivePoints <= 0 && studentRecommendConfig.BuyGiveMoney <= 0))
            {
                return;
            }
            var recommendStudentBucket = await _studentDAL.GetStudent(request.Student.RecommendStudentId.Value);
            if (recommendStudentBucket == null || recommendStudentBucket.Student == null)
            {
                LOG.Log.Error("[StudentRecommendRewardConsumerBuy]推荐的学员未找到", request, this.GetType());
                return;
            }
            var recommendStudent = recommendStudentBucket.Student;
            var now = DateTime.Now;
            if (studentRecommendConfig.BuyGivePoints > 0)
            {
                await _studentDAL.AddPoint(recommendStudent.Id, studentRecommendConfig.BuyGivePoints);
                await _studentPointsLogDAL.AddStudentPointsLog(new EtStudentPointsLog()
                {
                    IsDeleted = EmIsDeleted.Normal,
                    No = string.Empty,
                    Ot = now,
                    Points = studentRecommendConfig.BuyGivePoints,
                    Remark = $"推荐学员消费[{request.Student.Name},{request.Student.Phone}]奖励{studentRecommendConfig.BuyGivePoints}积分",
                    StudentId = recommendStudent.Id,
                    TenantId = recommendStudent.TenantId,
                    Type = EmStudentPointsLogType.RecommendStudentBuy
                });
            }
            if (studentRecommendConfig.BuyGiveMoney > 0)
            {
                var accountLog = await _studentAccountRechargeChangeBLL.GetStudentAccountRecharge(recommendStudent.Phone, recommendStudent.PhoneBak);
                if (accountLog == null)
                {
                    return;
                }
                await _studentAccountRechargeChangeBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(recommendStudent.TenantId)
                {
                    AddBalanceReal = 0,
                    AddBalanceGive = studentRecommendConfig.BuyGiveMoney,
                    AddRechargeSum = 0,
                    AddRechargeGiveSum = 0,
                    StudentAccountRechargeId = accountLog.Id,
                    TryCount = 0
                });

                await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
                {
                    CgNo = string.Empty,
                    CgBalanceReal = 0,
                    CgBalanceGive = studentRecommendConfig.BuyGiveMoney,
                    CgServiceCharge = 0,
                    CommissionUser = string.Empty,
                    IsDeleted = EmIsDeleted.Normal,
                    Ot = now,
                    Phone = accountLog.Phone,
                    RelatedOrderId = null,
                    Remark = $"推荐学员消费[{request.Student.Name},{request.Student.Phone}]奖励{studentRecommendConfig.BuyGiveMoney}金额",
                    Status = EmStudentAccountRechargeLogStatus.Normal,
                    StudentAccountRechargeId = accountLog.Id,
                    TenantId = accountLog.TenantId,
                    UserId = request.UserId,
                    Type = EmStudentAccountRechargeLogType.RecommendStudentBuy
                });
            }
        }
    }
}
