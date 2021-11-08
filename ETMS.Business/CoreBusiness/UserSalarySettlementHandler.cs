using ETMS.Entity.Common;
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

        private IEnumerable<EtTeacherSalaryClassTimes2> _teacherSalaryClassTimesList2;

        private List<long> _finishUserIds;

        private List<long> _failUserIds;

        private List<TeacherSalaryPayrollDetailView> _payrollUser;

        private EtTeacherSalaryPayroll _teacherSalaryPayroll;

        public UserSalarySettlementHandler(IUserDAL userDAL, ITeacherSalaryPayrollDAL teacherSalaryPayrollDAL,
            ITeacherSalaryContractDAL teacherSalaryContractDAL, bool isOpenContractPerformance,
            List<TeacherSalaryFundsItemOutput> allTeacherSalaryFundsItem, TeacherSalaryGlobalRuleView teacherSalaryGlobalRuleView,
            IEnumerable<EtTeacherSalaryClassTimes> teacherSalaryClassTimesList,
            IEnumerable<EtTeacherSalaryClassTimes2> teacherSalaryClassTimesList2)
        {
            this._userDAL = userDAL;
            this._teacherSalaryPayrollDAL = teacherSalaryPayrollDAL;
            this._teacherSalaryContractDAL = teacherSalaryContractDAL;
            this._isOpenContractPerformance = isOpenContractPerformance;
            this._allTeacherSalaryFundsItem = allTeacherSalaryFundsItem;
            this._globalConfig = teacherSalaryGlobalRuleView;
            this._teacherSalaryClassTimesList = teacherSalaryClassTimesList;
            this._teacherSalaryClassTimesList2 = teacherSalaryClassTimesList2;
            this._finishUserIds = new List<long>();
            this._failUserIds = new List<long>();
            this._payrollUser = new List<TeacherSalaryPayrollDetailView>();
        }

        private List<TeacherSalaryPayrollUserPerformanceView> GetUserPerformance(long userId,
            EtTeacherSalaryContractPerformanceSet myTeacherSalaryContractPerformanceSet,
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails,
            List<EtTeacherSalaryContractPerformanceLessonBasc> myTeacherSalaryContractPerformanceLessonBascs)
        {
            if (myTeacherSalaryContractPerformanceSetDetails == null ||
                myTeacherSalaryContractPerformanceSetDetails.Count == 0)
            {
                return new List<TeacherSalaryPayrollUserPerformanceView>();
            }
            var mySalaryClassTimesList = _teacherSalaryClassTimesList.Where(p => p.TeacherId == userId);
            if (!mySalaryClassTimesList.Any())
            {
                return new List<TeacherSalaryPayrollUserPerformanceView>();
            }
            var mySalaryClassTimesList2 = _teacherSalaryClassTimesList2.Where(p => p.TeacherId == userId);

            var processHandler = new UserSalaryPerformanceHandler(_teacherSalaryPayroll.TenantId, userId,
                myTeacherSalaryContractPerformanceSet, myTeacherSalaryContractPerformanceSetDetails, mySalaryClassTimesList2,
                mySalaryClassTimesList, _globalConfig, myTeacherSalaryContractPerformanceLessonBascs);
            var processMethod = typeof(UserSalaryPerformanceHandler).GetMethod($"Process_{_globalConfig.StatisticalRuleType}_{myTeacherSalaryContractPerformanceSet.ComputeType}_{myTeacherSalaryContractPerformanceSet.GradientCalculateType}");
            return processMethod.Invoke(processHandler, null) as List<TeacherSalaryPayrollUserPerformanceView>;
        }

        private async Task PayrollUserHandle(long userId)
        {
            var computeType = EmTeacherSalaryComputeType.Class;
            var gradientCalculateType = _globalConfig.GradientCalculateType;
            List<EtTeacherSalaryContractFixed> myTeacherSalaryContractFixeds = null;
            EtTeacherSalaryContractPerformanceSet myTeacherSalaryContractPerformanceSet = null;
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails = null;
            List<EtTeacherSalaryContractPerformanceLessonBasc> myTeacherSalaryContractPerformanceLessonBascs = null;
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
                if (myTeacherSalaryContractSetBucket.EtTeacherSalaryContractPerformanceLessonBascs != null
                    && myTeacherSalaryContractSetBucket.EtTeacherSalaryContractPerformanceLessonBascs.Any())
                {
                    myTeacherSalaryContractPerformanceLessonBascs = myTeacherSalaryContractSetBucket.EtTeacherSalaryContractPerformanceLessonBascs;
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
                    IncludeArrivedMakeUpStudent = _globalConfig.IncludeArrivedMakeUpStudent,
                    IncludeArrivedTryCalssStudent = _globalConfig.IncludeArrivedTryCalssStudent,
                    PerformanceSetDesc = myTeacherSalaryContractPerformanceSet?.ComputeDesc
                },
                TeacherSalaryPayrollUserDetails = new List<EtTeacherSalaryPayrollUserDetail>(),
                PerformanceViews = new List<TeacherSalaryPayrollUserPerformanceView>()
            };
            var index = 0;
            foreach (var myFundsItem in _allTeacherSalaryFundsItem)
            {
                if (myFundsItem.Id == SystemConfig.ComConfig.TeacherSalaryPerformanceDefaultId) //绩效工资
                {
                    var amountSum1 = 0M;
                    if (myTeacherSalaryContractPerformanceSetDetails != null && myTeacherSalaryContractPerformanceSetDetails.Any())
                    {
                        myPayrollUser.PerformanceViews = GetUserPerformance(userId, myTeacherSalaryContractPerformanceSet,
                            myTeacherSalaryContractPerformanceSetDetails, myTeacherSalaryContractPerformanceLessonBascs);
                        if (myPayrollUser.PerformanceViews.Any())
                        {
                            amountSum1 = myPayrollUser.PerformanceViews.Sum(p => p.TeacherSalaryPayrollUserPerformance.SubmitSum);
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
            var tempPayItemSum = 0M;
            foreach (var p in myPayrollUser.TeacherSalaryPayrollUserDetails)
            {
                if (p.FundsItemsType == EmTeacherSalaryFundsItemsType.Add)
                {
                    tempPayItemSum += p.AmountSum;
                }
                else
                {
                    tempPayItemSum -= p.AmountSum;
                }
            }
            myPayrollUser.TeacherSalaryPayrollUser.PayItemSum = tempPayItemSum;
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
                var performances = p.PerformanceViews;
                foreach (var u in userDetails)
                {
                    u.TeacherSalaryPayrollId = teacherSalaryPayrollId;
                    u.TeacherSalaryPayrollUserId = teacherSalaryPayrollUserId;
                }
                _teacherSalaryPayrollDAL.AddTeacherSalaryPayrollDetail(userDetails);

                if (performances != null && performances.Count > 0)
                {
                    foreach (var f in performances)
                    {
                        var userPerformance = f.TeacherSalaryPayrollUserPerformance;
                        userPerformance.TeacherSalaryPayrollId = teacherSalaryPayrollId;
                        userPerformance.TeacherSalaryPayrollUserId = teacherSalaryPayrollUserId;
                        var userPerformanceId = await _teacherSalaryPayrollDAL.AddTeacherSalaryPayrollUserPerformance(userPerformance);

                        var userPerformanceDetails = f.PerformanceDetails;
                        if (userPerformanceDetails != null && userPerformanceDetails.Count > 0)
                        {
                            foreach (var j in userPerformanceDetails)
                            {
                                j.TeacherSalaryPayrollId = teacherSalaryPayrollId;
                                j.TeacherSalaryPayrollUserId = teacherSalaryPayrollUserId;
                                j.UserPerformanceId = userPerformanceId;
                            }
                            _teacherSalaryPayrollDAL.AddTeacherSalaryPayrollUserPerformanceDetail(userPerformanceDetails);
                        }
                    }
                }
            }

            var output = new TeacherSalaryPayrollGoSettlementOutput()
            {
                CId = teacherSalaryPayrollId,
                FinishCount = _finishUserIds.Count,
                FailCount = _failUserIds.Count
            };
            return ResponseBase.Success(output);
        }
    }
}
