﻿using ETMS.Entity.Common;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Teacher.Output;
using ETMS.Entity.Dto.Teacher.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using ETMS.IDataAccess;
using ETMS.IDataAccess.TeacherSalary;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    public class UserSalarySettlementHandler
    {
        private readonly IUserDAL _userDAL;

        private readonly ITeacherSalaryPayrollDAL _teacherSalaryPayrollDAL;

        private readonly ITeacherSalaryContractDAL _teacherSalaryContractDAL;

        private bool _isOpenContractPerformance;

        private List<TeacherSalaryFundsItemOutput> _allTeacherSalaryFundsItem;

        private TeacherSalaryGlobalRuleView _globalConfig;

        private IEnumerable<EtTeacherSalaryClassTimes> _teacherSalaryClassTimesList;

        private List<long> _finishUserIds;

        private List<long> _failUserIds;

        private List<TeacherSalaryPayrollDetailView> _payrollUser;

        private EtTeacherSalaryPayroll _teacherSalaryPayroll;

        public UserSalarySettlementHandler(IUserDAL userDAL, ITeacherSalaryPayrollDAL teacherSalaryPayrollDAL,
            ITeacherSalaryContractDAL teacherSalaryContractDAL, bool isOpenContractPerformance,
            List<TeacherSalaryFundsItemOutput> allTeacherSalaryFundsItem, TeacherSalaryGlobalRuleView teacherSalaryGlobalRuleView,
            IEnumerable<EtTeacherSalaryClassTimes> teacherSalaryClassTimesList)
        {
            this._userDAL = userDAL;
            this._teacherSalaryPayrollDAL = teacherSalaryPayrollDAL;
            this._teacherSalaryContractDAL = teacherSalaryContractDAL;
            this._isOpenContractPerformance = isOpenContractPerformance;
            this._allTeacherSalaryFundsItem = allTeacherSalaryFundsItem;
            this._globalConfig = teacherSalaryGlobalRuleView;
            this._teacherSalaryClassTimesList = teacherSalaryClassTimesList;
            this._finishUserIds = new List<long>();
            this._failUserIds = new List<long>();
            this._payrollUser = new List<TeacherSalaryPayrollDetailView>();
        }

        private List<EtTeacherSalaryPayrollUserPerformance> GetUserPerformance(long userId,
            EtTeacherSalaryContractPerformanceSet myTeacherSalaryContractPerformanceSet,
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails)
        {
            if (myTeacherSalaryContractPerformanceSetDetails == null ||
                myTeacherSalaryContractPerformanceSetDetails.Count == 0)
            {
                return new List<EtTeacherSalaryPayrollUserPerformance>();
            }
            var mySalaryClassTimesList = _teacherSalaryClassTimesList.Where(p => p.TeacherId == userId);
            if (!mySalaryClassTimesList.Any())
            {
                return new List<EtTeacherSalaryPayrollUserPerformance>();
            }

            var processHandler = new UserSalaryPerformanceHandler(_teacherSalaryPayroll.TenantId, userId,
                myTeacherSalaryContractPerformanceSet, myTeacherSalaryContractPerformanceSetDetails, mySalaryClassTimesList, _globalConfig);
            var processMethod = typeof(UserSalaryPerformanceHandler).GetMethod($"Process_{_globalConfig.StatisticalRuleType}_{myTeacherSalaryContractPerformanceSet.ComputeType}_{myTeacherSalaryContractPerformanceSet.GradientCalculateType}");
            return processMethod.Invoke(processHandler, null) as List<EtTeacherSalaryPayrollUserPerformance>;
        }

        private async Task PayrollUserHandle(long userId)
        {
            var computeType = EmTeacherSalaryComputeType.Class;
            var gradientCalculateType = _globalConfig.GradientCalculateType;
            List<EtTeacherSalaryContractFixed> myTeacherSalaryContractFixeds = null;
            EtTeacherSalaryContractPerformanceSet myTeacherSalaryContractPerformanceSet = null;
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails = null;
            var myTeacherSalaryContractSetBucket = await _teacherSalaryContractDAL.GetTeacherSalaryContract(userId);
            if (myTeacherSalaryContractSetBucket != null)
            {
                if (myTeacherSalaryContractSetBucket.TeacherSalaryContractFixeds != null &&
                    myTeacherSalaryContractSetBucket.TeacherSalaryContractFixeds.Any())
                {
                    myTeacherSalaryContractFixeds = myTeacherSalaryContractSetBucket.TeacherSalaryContractFixeds;
                }
                if (myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet != null)
                {
                    myTeacherSalaryContractPerformanceSet = myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSet;
                }
                if (myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails != null &&
                    myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails.Any())
                {
                    myTeacherSalaryContractPerformanceSetDetails = myTeacherSalaryContractSetBucket.TeacherSalaryContractPerformanceSetDetails;
                }
            }
            if (myTeacherSalaryContractPerformanceSet != null)
            {
                computeType = myTeacherSalaryContractPerformanceSet.ComputeType;
                gradientCalculateType = myTeacherSalaryContractPerformanceSet.GradientCalculateType;
            }
            var myPayrollUser = new TeacherSalaryPayrollDetailView()
            {
                TeacherSalaryPayrollUser = new EtTeacherSalaryPayrollUser()
                {
                    ComputeType = computeType,
                    GradientCalculateType = gradientCalculateType,
                    EndDate = _teacherSalaryPayroll.EndDate,
                    IsDeleted = _teacherSalaryPayroll.IsDeleted,
                    Name = _teacherSalaryPayroll.Name,
                    OpUserId = _teacherSalaryPayroll.OpUserId,
                    UserId = userId,
                    TenantId = _teacherSalaryPayroll.TenantId,
                    Ot = _teacherSalaryPayroll.Ot,
                    PayDate = _teacherSalaryPayroll.PayDate,
                    StartDate = _teacherSalaryPayroll.StartDate,
                    Status = _teacherSalaryPayroll.Status,
                    StatisticalRuleType = _globalConfig.StatisticalRuleType,
                    PerformanceSetDesc = myTeacherSalaryContractPerformanceSet?.ComputeDesc
                },
                TeacherSalaryPayrollUserDetails = new List<EtTeacherSalaryPayrollUserDetail>(),
                TeacherSalaryPayrollUserPerformances = new List<EtTeacherSalaryPayrollUserPerformance>()
            };
            var index = 0;
            foreach (var myFundsItem in _allTeacherSalaryFundsItem)
            {
                if (myFundsItem.Id == SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId) //绩效工资
                {
                    var amountSum1 = 0M;
                    if (myTeacherSalaryContractPerformanceSetDetails != null && myTeacherSalaryContractPerformanceSetDetails.Any())
                    {
                        myPayrollUser.TeacherSalaryPayrollUserPerformances = GetUserPerformance(userId, myTeacherSalaryContractPerformanceSet, myTeacherSalaryContractPerformanceSetDetails);
                        if (myPayrollUser.TeacherSalaryPayrollUserPerformances.Any())
                        {
                            amountSum1 = myPayrollUser.TeacherSalaryPayrollUserPerformances.Sum(p => p.SubmitSum);
                        }
                    }
                    myPayrollUser.TeacherSalaryPayrollUserDetails.Add(new EtTeacherSalaryPayrollUserDetail()
                    {
                        AmountSum = amountSum1,
                        FundsItemsId = myFundsItem.Id,
                        FundsItemsType = myFundsItem.Type,
                        FundsItemsName = myFundsItem.Name,
                        IsDeleted = EmIsDeleted.Normal,
                        IsPerformance = EmBool.True,
                        OrderIndex = index,
                        TenantId = _teacherSalaryPayroll.TenantId,
                        UserId = userId
                    });
                }
                else
                {
                    var amountSum2 = 0M;
                    if (myTeacherSalaryContractFixeds != null && myTeacherSalaryContractFixeds.Any())
                    {
                        var myContractFixed = myTeacherSalaryContractFixeds.FirstOrDefault(p => p.FundsItemsId == myFundsItem.Id);
                        if (myContractFixed != null)
                        {
                            amountSum2 = myContractFixed.AmountValue;
                        }
                    }
                    myPayrollUser.TeacherSalaryPayrollUserDetails.Add(new EtTeacherSalaryPayrollUserDetail()
                    {
                        AmountSum = amountSum2,
                        FundsItemsId = myFundsItem.Id,
                        FundsItemsType = myFundsItem.Type,
                        FundsItemsName = myFundsItem.Name,
                        IsDeleted = EmIsDeleted.Normal,
                        IsPerformance = EmBool.False,
                        OrderIndex = index,
                        TenantId = _teacherSalaryPayroll.TenantId,
                        UserId = userId
                    });
                }
            }
            myPayrollUser.TeacherSalaryPayrollUser.PayItemSum = myPayrollUser.TeacherSalaryPayrollUserDetails.Sum(p => p.AmountSum);
            _payrollUser.Add(myPayrollUser);
        }

        public async Task<ResponseBase> Process(TeacherSalaryPayrollGoSettlementRequest request)
        {
            this._teacherSalaryPayroll = new EtTeacherSalaryPayroll()
            {
                StartDate = request.StartOt.Value,
                EndDate = request.EndOt.Value,
                Ot = DateTime.Now,
                IsDeleted = EmIsDeleted.Normal,
                Name = request.Name,
                OpUserId = request.LoginUserId,
                PayDate = request.PayDate,
                Status = EmTeacherSalaryPayrollStatus.NotSure,
                TenantId = request.LoginTenantId
            };
            foreach (var userId in request.UserIds)
            {
                var user = await _userDAL.GetUser(userId);
                if (user == null)
                {
                    _failUserIds.Add(userId);
                    continue;
                }
                await PayrollUserHandle(userId);
                _finishUserIds.Add(userId);
            }
            if (_finishUserIds.Count == 0)
            {
                return ResponseBase.CommonError("结算失败，员工无效");
            }

            this._teacherSalaryPayroll.UserIds = EtmsHelper.GetMuIds(_finishUserIds);
            this._teacherSalaryPayroll.UserCount = _finishUserIds.Count;
            this._teacherSalaryPayroll.PaySum = _payrollUser.Sum(p => p.TeacherSalaryPayrollUser.PayItemSum);
            var teacherSalaryPayrollId = await _teacherSalaryPayrollDAL.AddTeacherSalaryPayroll(this._teacherSalaryPayroll);
            foreach (var p in _payrollUser)
            {
                var teacherSalaryPayrollUser = p.TeacherSalaryPayrollUser;
                teacherSalaryPayrollUser.TeacherSalaryPayrollId = teacherSalaryPayrollId;
                var teacherSalaryPayrollUserId = await _teacherSalaryPayrollDAL.AddTeacherSalaryPayrollUser(teacherSalaryPayrollUser);

                var userDetails = p.TeacherSalaryPayrollUserDetails;
                var performances = p.TeacherSalaryPayrollUserPerformances;
                foreach (var u in userDetails)
                {
                    u.TeacherSalaryPayrollId = teacherSalaryPayrollId;
                    u.TeacherSalaryPayrollUserId = teacherSalaryPayrollUserId;
                }
                foreach (var f in performances)
                {
                    f.TeacherSalaryPayrollId = teacherSalaryPayrollId;
                    f.TeacherSalaryPayrollUserId = teacherSalaryPayrollUserId;
                }
                _teacherSalaryPayrollDAL.AddTeacherSalaryPayrollDetail(userDetails, performances);
            }

            var output = new TeacherSalaryPayrollGoSettlementOutput()
            {
                CId = teacherSalaryPayrollId,
                FinishCount = _finishUserIds.Count,
                FailCount = _failUserIds.Count
            };
            return ResponseBase.Success();
        }
    }
}
