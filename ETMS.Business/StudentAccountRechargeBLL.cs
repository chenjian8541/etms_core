using ETMS.Business.Common;
using ETMS.Entity.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Student.Output;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.IBusiness;
using ETMS.IDataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ETMS.Business
{
    public class StudentAccountRechargeBLL : IStudentAccountRechargeBLL
    {
        private readonly IAppConfigDAL _appConfigDAL;

        private readonly IUserOperationLogDAL _userOperationLogDAL;

        private readonly IStatisticsStudentAccountRechargeDAL _statisticsStudentAccountRechargeDAL;

        private readonly IStudentAccountRechargeDAL _studentAccountRechargeDAL;

        private readonly IStudentAccountRechargeLogDAL _studentAccountRechargeLogDAL;

        private readonly IUserDAL _userDAL;

        private readonly IParentStudentDAL _parentStudentDAL;

        public StudentAccountRechargeBLL(IAppConfigDAL appConfigDAL, IUserOperationLogDAL userOperationLogDAL,
            IStatisticsStudentAccountRechargeDAL statisticsStudentAccountRechargeDAL, IStudentAccountRechargeDAL studentAccountRechargeDAL,
            IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL, IUserDAL userDAL, IParentStudentDAL parentStudentDAL)
        {
            this._appConfigDAL = appConfigDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._statisticsStudentAccountRechargeDAL = statisticsStudentAccountRechargeDAL;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._userDAL = userDAL;
            this._parentStudentDAL = parentStudentDAL;
        }

        public void InitTenantId(int tenantId)
        {
            this.InitDataAccess(tenantId, _appConfigDAL, _userOperationLogDAL, _statisticsStudentAccountRechargeDAL, _studentAccountRechargeDAL,
                _studentAccountRechargeLogDAL, _userDAL, _parentStudentDAL);
        }

        public async Task<ResponseBase> StudentAccountRechargeRuleGet(StudentAccountRechargeRuleGetRequest request)
        {
            var log = await this._appConfigDAL.GetAppConfig(EmAppConfigType.RechargeRuleConfig);
            if (log == null)
            {
                return ResponseBase.Success(new StudentAccountRechargeRuleView());
            }
            var rechargeRuleView = JsonConvert.DeserializeObject<StudentAccountRechargeRuleView>(log.ConfigValue);
            return ResponseBase.Success(rechargeRuleView);
        }

        public async Task<ResponseBase> StudentAccountRechargeRuleSave(StudentAccountRechargeRuleSaveRequest request)
        {
            var configModel = new StudentAccountRechargeRuleView()
            {
                Explain = request.Explain,
                ImgUrlKey = request.ImgUrlKey
            };
            var config = new EtAppConfig()
            {
                ConfigValue = JsonConvert.SerializeObject(configModel),
                IsDeleted = EmIsDeleted.Normal,
                Remark = string.Empty,
                TenantId = request.LoginTenantId,
                Type = EmAppConfigType.RechargeRuleConfig
            };
            await this._appConfigDAL.SaveAppConfig(config);

            await _userOperationLogDAL.AddUserLog(request, "充值规则设置", EmUserOperationType.StudentAccountRechargeManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StatisticsStudentAccountRechargeGet(StatisticsStudentAccountRechargeGetRequest request)
        {
            var accountLog = await _statisticsStudentAccountRechargeDAL.GetStatisticsStudentAccountRecharge();
            if (accountLog == null)
            {
                return ResponseBase.Success(new StatisticsStudentAccountRechargeGetOutput());
            }
            return ResponseBase.Success(new StatisticsStudentAccountRechargeGetOutput()
            {
                AccountCount = accountLog.AccountCount,
                BalanceGive = accountLog.BalanceGive,
                BalanceReal = accountLog.BalanceReal,
                BalanceSum = accountLog.BalanceSum,
                RechargeGiveSum = accountLog.RechargeGiveSum,
                RechargeSum = accountLog.RechargeSum
            });
        }

        public async Task<ResponseBase> StudentAccountRechargeLogGetPaging(StudentAccountRechargeLogGetPagingRequest request)
        {
            var pagingData = await _studentAccountRechargeLogDAL.GetPaging(request);
            var output = new List<StudentAccountRechargeLogGetPagingOutput>();
            var tempBoxUser = new DataTempBox<EtUser>();
            foreach (var p in pagingData.Item1)
            {
                output.Add(new StudentAccountRechargeLogGetPagingOutput()
                {
                    CgBalanceGive = p.CgBalanceGive,
                    CgBalanceReal = p.CgBalanceReal,
                    CgNo = p.CgNo,
                    CgServiceCharge = p.CgServiceCharge,
                    CommissionUser = p.CommissionUser,
                    CommissionUserDesc = await ComBusiness.GetUserNames(tempBoxUser, _userDAL, p.CommissionUser),
                    UserId = p.UserId,
                    UserDesc = await ComBusiness.GetUserName(tempBoxUser, _userDAL, p.UserId),
                    Ot = p.Ot,
                    Phone = p.Phone,
                    RelatedOrderId = p.RelatedOrderId,
                    Remark = p.Remark,
                    Status = p.Status,
                    StudentAccountRechargeId = p.StudentAccountRechargeId,
                    Type = p.Type,
                    TypeDesc = EmStudentAccountRechargeLogType.GetStudentAccountRechargeLogTypeDesc(p.Type)
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentAccountRechargeLogGetPagingOutput>(pagingData.Item2, output));
        }

        public async Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeRequest request)
        {
            var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.Id);
            if (accountLog == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            return ResponseBase.Success(new StudentAccountRechargeGetOutput()
            {
                BalanceGive = accountLog.BalanceGive,
                Id = accountLog.Id,
                BalanceReal = accountLog.BalanceReal,
                BalanceSum = accountLog.BalanceSum,
                Ot = accountLog.Ot,
                Phone = accountLog.Phone,
                RechargeGiveSum = accountLog.RechargeGiveSum,
                RechargeSum = accountLog.RechargeSum
            });
        }

        public async Task<ResponseBase> StudentAccountRechargeGetPaging(StudentAccountRechargeGetPagingRequest request)
        {
            var output = new List<StudentAccountRechargeGetPagingOutput>();
            var pagingData = await _studentAccountRechargeDAL.GetPaging(request);
            foreach (var p in pagingData.Item1)
            {
                var parentStudents = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, p.Phone);
                var pelationStudent = string.Empty;
                if (parentStudents != null && parentStudents.Any())
                {
                    pelationStudent = string.Join(',', parentStudents);
                }
                output.Add(new StudentAccountRechargeGetPagingOutput()
                {
                    Phone = p.Phone,
                    BalanceGive = p.BalanceGive,
                    BalanceReal = p.BalanceReal,
                    BalanceSum = p.BalanceSum,
                    Ot = p.Ot,
                    RechargeGiveSum = p.RechargeGiveSum,
                    RechargeSum = p.RechargeSum,
                    Id = p.Id,
                    RelationStudent = pelationStudent
                });
            }
            return ResponseBase.Success(new ResponsePagingDataBase<StudentAccountRechargeGetPagingOutput>(pagingData.Item2, output));
        }
    }
}
