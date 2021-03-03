﻿using ETMS.Business.Common;
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
using ETMS.IEventProvider;
using ETMS.Event.DataContract;
using ETMS.Utility;
using ETMS.Event.DataContract.Statistics;

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

        private readonly IEventPublisher _eventPublisher;

        private readonly IStudentPointsLogDAL _studentPointsLogDAL;

        private readonly IStudentDAL _studentDAL;

        private readonly IOrderDAL _orderDAL;

        private readonly IIncomeLogDAL _incomeLogDAL;

        private readonly IStudentAccountRechargeChangeBLL _studentAccountRechargeChangeBLL;

        public StudentAccountRechargeBLL(IAppConfigDAL appConfigDAL, IUserOperationLogDAL userOperationLogDAL,
            IStatisticsStudentAccountRechargeDAL statisticsStudentAccountRechargeDAL, IStudentAccountRechargeDAL studentAccountRechargeDAL,
            IStudentAccountRechargeLogDAL studentAccountRechargeLogDAL, IUserDAL userDAL, IParentStudentDAL parentStudentDAL,
            IEventPublisher eventPublisher, IStudentPointsLogDAL studentPointsLogDAL, IStudentDAL studentDAL, IOrderDAL orderDAL,
            IIncomeLogDAL incomeLogDAL, IStudentAccountRechargeChangeBLL studentAccountRechargeChangeBLL)
        {
            this._appConfigDAL = appConfigDAL;
            this._userOperationLogDAL = userOperationLogDAL;
            this._statisticsStudentAccountRechargeDAL = statisticsStudentAccountRechargeDAL;
            this._studentAccountRechargeDAL = studentAccountRechargeDAL;
            this._studentAccountRechargeLogDAL = studentAccountRechargeLogDAL;
            this._userDAL = userDAL;
            this._parentStudentDAL = parentStudentDAL;
            this._eventPublisher = eventPublisher;
            this._studentPointsLogDAL = studentPointsLogDAL;
            this._studentDAL = studentDAL;
            this._orderDAL = orderDAL;
            this._incomeLogDAL = incomeLogDAL;
            this._studentAccountRechargeChangeBLL = studentAccountRechargeChangeBLL;
        }

        public void InitTenantId(int tenantId)
        {
            this._studentAccountRechargeChangeBLL.InitTenantId(tenantId);
            this.InitDataAccess(tenantId, _appConfigDAL, _userOperationLogDAL, _statisticsStudentAccountRechargeDAL, _studentAccountRechargeDAL,
                _studentAccountRechargeLogDAL, _userDAL, _parentStudentDAL, _studentPointsLogDAL, _studentDAL, _orderDAL, _incomeLogDAL);
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

        public async Task<ResponseBase> StudentAccountRechargeGet(StudentAccountRechargeGetRequest request)
        {
            var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.Id);
            if (accountLog == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            var parentStudents = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, accountLog.Phone);
            var pelationStudentStr = string.Empty;
            if (parentStudents != null && parentStudents.Any())
            {
                pelationStudentStr = string.Join(',', parentStudents);
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
                RechargeSum = accountLog.RechargeSum,
                PelationStudent = pelationStudentStr
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

        public async Task<ResponseBase> StudentAccountRechargeGetByPhone(StudentAccountRechargeGetByPhoneRequest request)
        {
            var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.Phone);
            if (accountLog == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            var parentStudents = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, accountLog.Phone);
            var pelationStudentStr = string.Empty;
            if (parentStudents != null && parentStudents.Any())
            {
                pelationStudentStr = string.Join(',', parentStudents);
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
                RechargeSum = accountLog.RechargeSum,
                PelationStudent = pelationStudentStr
            });
        }

        public async Task<ResponseBase> StudentAccountRechargeCreate(StudentAccountRechargeCreateRequest request)
        {
            if (await _studentAccountRechargeDAL.ExistStudentAccountRecharge(request.Phone))
            {
                return ResponseBase.CommonError("账户已存在");
            }

            await _studentAccountRechargeDAL.AddStudentAccountRecharge(new EtStudentAccountRecharge()
            {
                BalanceSum = 0,
                Phone = request.Phone,
                BalanceGive = 0,
                BalanceReal = 0,
                IsDeleted = EmIsDeleted.Normal,
                Ot = DateTime.Now,
                RechargeGiveSum = 0,
                RechargeSum = 0,
                TenantId = request.LoginTenantId
            });

            await _userOperationLogDAL.AddUserLog(request, $"创建充值账户-{request.Phone}", EmUserOperationType.StudentAccountRechargeManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentAccountRechargeEditPhone(StudentAccountRechargeEditPhoneRequest request)
        {
            if (await _studentAccountRechargeDAL.ExistStudentAccountRecharge(request.Phone, request.Id))
            {
                return ResponseBase.CommonError("此手机号已存在账户信息");
            }

            var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.Id);
            if (accountLog == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            if (accountLog.Phone == request.Phone)
            {
                return ResponseBase.CommonError("账户手机号码相同，无需修改");
            }

            accountLog.Phone = request.Phone;
            await _studentAccountRechargeDAL.EditStudentAccountRecharge(accountLog);

            _eventPublisher.Publish(new SyncStudentAccountRechargeLogPhoneEvent(request.LoginTenantId)
            {
                StudentAccountRechargeId = accountLog.Id,
                NewPhone = request.Phone
            });
            await _userOperationLogDAL.AddUserLog(request, $"编辑充值账户手机号码-{request.Phone}", EmUserOperationType.StudentAccountRechargeManage);
            return ResponseBase.Success();
        }

        public async Task<ResponseBase> StudentAccountRecharge(StudentAccountRechargeRequest request)
        {
            var accountLog = await _studentAccountRechargeDAL.GetStudentAccountRecharge(request.StudentAccountRechargeId);
            if (accountLog == null)
            {
                return ResponseBase.CommonError("账户不存在");
            }
            var no = OrderNumberLib.StudentAccountRecharge();
            var now = DateTime.Now;
            await _studentAccountRechargeChangeBLL.StudentAccountRechargeChange(new StudentAccountRechargeChangeEvent(request.LoginTenantId)
            {
                AddBalanceReal = request.RechargeReal,
                AddBalanceGive = request.RechargeGive,
                AddRechargeSum = request.RechargeReal,
                AddRechargeGiveSum = request.RechargeGive,
                StudentAccountRechargeId = request.StudentAccountRechargeId,
                TryCount = 0
            });

            _eventPublisher.Publish(new StudentAccountRechargeEvent(request.LoginTenantId)
            {
                AccountLog = accountLog,
                RechargeRequest = request,
                No = no,
                CreateOt = now
            });

            return ResponseBase.Success();
        }

        public async Task StudentAccountRechargeConsumerEvent(StudentAccountRechargeEvent eventRequest)
        {
            var now = eventRequest.CreateOt;
            var no = eventRequest.No;
            var request = eventRequest.RechargeRequest;
            var accountLog = eventRequest.AccountLog;
            var incomeLogs = new List<EtIncomeLog>();
            if (request.PayInfo.PayWechat > 0)
            {
                incomeLogs.Add(GetStudentAccountRechargeIncomeLog(EmPayType.WeChat, request.PayInfo.PayWechat, now, request.Ot, no, request));
            }
            if (request.PayInfo.PayAlipay > 0)
            {
                incomeLogs.Add(GetStudentAccountRechargeIncomeLog(EmPayType.Alipay, request.PayInfo.PayAlipay, now, request.Ot, no, request));
            }
            if (request.PayInfo.PayCash > 0)
            {
                incomeLogs.Add(GetStudentAccountRechargeIncomeLog(EmPayType.Cash, request.PayInfo.PayCash, now, request.Ot, no, request));
            }
            if (request.PayInfo.PayBank > 0)
            {
                incomeLogs.Add(GetStudentAccountRechargeIncomeLog(EmPayType.Bank, request.PayInfo.PayBank, now, request.Ot, no, request));
            }
            if (request.PayInfo.PayPos > 0)
            {
                incomeLogs.Add(GetStudentAccountRechargeIncomeLog(EmPayType.Pos, request.PayInfo.PayPos, now, request.Ot, no, request));
            }
            var paySum = request.PayInfo.PaySum;
            var aptSum = paySum;
            var order = new EtOrder()
            {
                AptSum = aptSum,
                PaySum = paySum,
                ArrearsSum = 0,
                BuyCost = string.Empty,
                BuyCourse = string.Empty,
                BuyGoods = string.Empty,
                BuyOther = "账户充值",
                CommissionUser = EtmsHelper.GetMuIds(request.CommissionUser),
                CouponsIds = string.Empty,
                CouponsStudentGetIds = string.Empty,
                CreateOt = now,
                Ot = request.Ot,
                InOutType = EmOrderInOutType.In,
                IsDeleted = EmIsDeleted.Normal,
                IsReturn = EmBool.False,
                IsTransferCourse = EmBool.False,
                No = no,
                OrderType = EmOrderType.StudentAccountRecharge,
                Remark = request.Remark,
                Status = EmOrderStatus.Normal,
                StudentId = 0,
                StudentAccountRechargeId = request.StudentAccountRechargeId,
                Sum = aptSum,
                TenantId = request.LoginTenantId,
                TotalPoints = request.TotalPoints,
                UnionOrderId = null,
                UnionOrderNo = null,
                UnionTransferOrderIds = string.Empty,
                UserId = request.LoginUserId
            };

            //关联的学员 将赠送相应的积分
            if (request.TotalPoints > 0)
            {
                var parentStudents = await _parentStudentDAL.GetParentStudents(request.LoginTenantId, accountLog.Phone);
                if (parentStudents != null && parentStudents.Any())
                {
                    var studentPointsLogs = new List<EtStudentPointsLog>();
                    foreach (var p in parentStudents)
                    {
                        studentPointsLogs.Add(new EtStudentPointsLog()
                        {
                            IsDeleted = EmIsDeleted.Normal,
                            No = order.No,
                            Ot = now,
                            Points = request.TotalPoints,
                            Remark = string.Empty,
                            StudentId = p.Id,
                            TenantId = order.TenantId,
                            Type = EmStudentPointsLogType.StudentAccountRecharge
                        });
                        await _studentDAL.AddPoint(p.Id, request.TotalPoints);
                    }
                    if (studentPointsLogs.Count > 0)
                    {
                        _studentPointsLogDAL.AddStudentPointsLog(studentPointsLogs);
                    }
                }
            }

            var orderId = await _orderDAL.AddOrder(order);
            if (incomeLogs.Any())
            {
                foreach (var p in incomeLogs)
                {
                    p.OrderId = orderId;
                }
                _incomeLogDAL.AddIncomeLog(incomeLogs);
            }

            await _studentAccountRechargeLogDAL.AddStudentAccountRechargeLog(new EtStudentAccountRechargeLog()
            {
                UserId = request.LoginUserId,
                StudentAccountRechargeId = request.StudentAccountRechargeId,
                Phone = accountLog.Phone,
                CgBalanceGive = request.RechargeGive,
                CgBalanceReal = request.RechargeReal,
                CgNo = no,
                CgServiceCharge = 0,
                CommissionUser = order.CommissionUser,
                IsDeleted = EmIsDeleted.Normal,
                Ot = now,
                RelatedOrderId = orderId,
                Remark = order.Remark,
                Status = EmStudentAccountRechargeLogStatus.Normal,
                TenantId = order.TenantId,
                Type = EmStudentAccountRechargeLogType.Recharge
            });

            _eventPublisher.Publish(new StatisticsFinanceIncomeEvent(request.LoginTenantId)
            {
                StatisticsDate = order.Ot
            });

            await _userOperationLogDAL.AddUserLog(request, $"账户充值-账户:{accountLog.Phone},实充金额:{request.RechargeReal},赠送金额:{request.RechargeGive}", EmUserOperationType.StudentAccountRechargeManage);
        }

        private EtIncomeLog GetStudentAccountRechargeIncomeLog(byte payType, decimal payValue, DateTime createTime,
            DateTime ot, string no, RequestBase request)
        {
            return new EtIncomeLog()
            {
                AccountNo = string.Empty,
                IsDeleted = EmIsDeleted.Normal,
                No = no,
                Ot = ot,
                PayType = payType,
                ProjectType = EmIncomeLogProjectType.StudentAccountRecharge,
                Remark = string.Empty,
                RepealOt = null,
                OrderId = null,
                RepealUserId = null,
                Status = EmIncomeLogStatus.Normal,
                Sum = payValue,
                TenantId = request.LoginTenantId,
                Type = EmIncomeLogType.AccountIn,
                UserId = request.LoginUserId,
                CreateOt = createTime
            };
        }

        public async Task<ResponseBase> StudentAccountRefund(StudentAccountRefundRequest request)
        {
            return ResponseBase.Success();
        }
    }
}