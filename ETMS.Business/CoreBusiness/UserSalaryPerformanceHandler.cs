using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
using ETMS.Entity.Temp;
using ETMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business
{
    /// <summary>
    /// 按照不同规则进行 员工绩效计算    (可以考虑使用接口实现，工厂模式)
    /// 统计规则：按合计值统计、按每个课次单独统计
    /// 结算方式：按班级设置、按课程设置、统一设置
    /// 梯度计算：无梯度 、超额累计、全额累计
    /// 
    /// 注：以下public方法的方法名不能修改 ,因为调用方是通过反射进行调用的
    /// </summary>
    public class UserSalaryPerformanceHandler
    {
        private int _tenantId;
        private long _userId;
        private EtTeacherSalaryContractPerformanceSet _myTeacherSalaryContractPerformanceSet;
        private List<EtTeacherSalaryContractPerformanceSetDetail> _myTeacherSalaryContractPerformanceSetDetails;
        private IEnumerable<EtTeacherSalaryClassTimes2> _mySalaryClassTimesList2;
        private IEnumerable<EtTeacherSalaryClassTimes> _mySalaryClassTimesList;
        private TeacherSalaryGlobalRuleView _globalConfig;
        private List<EtTeacherSalaryContractPerformanceLessonBasc> _myTeacherSalaryContractPerformanceLessonBascs;
        public UserSalaryPerformanceHandler(int tenantId, long userId, EtTeacherSalaryContractPerformanceSet myTeacherSalaryContractPerformanceSet,
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails,
            IEnumerable<EtTeacherSalaryClassTimes2> mySalaryClassTimesList2, IEnumerable<EtTeacherSalaryClassTimes> mySalaryClassTimesList,
            TeacherSalaryGlobalRuleView teacherSalaryGlobalRuleView
            , List<EtTeacherSalaryContractPerformanceLessonBasc> myTeacherSalaryContractPerformanceLessonBascs)
        {
            this._tenantId = tenantId;
            this._userId = userId;
            this._myTeacherSalaryContractPerformanceSet = myTeacherSalaryContractPerformanceSet;
            this._myTeacherSalaryContractPerformanceSetDetails = myTeacherSalaryContractPerformanceSetDetails;
            this._mySalaryClassTimesList2 = mySalaryClassTimesList2;
            this._mySalaryClassTimesList = mySalaryClassTimesList;
            this._globalConfig = teacherSalaryGlobalRuleView;
            this._myTeacherSalaryContractPerformanceLessonBascs = myTeacherSalaryContractPerformanceLessonBascs;
        }

        #region 计算绩效值

        /// <summary>
        /// 获取有效的到课人数
        /// </summary>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private int GetValidArrivedAndBeLateCount<T>(IEnumerable<T> myClassTimes) where T : BaseTeacherSalaryClassTimes
        {
            var totalValidArrivedAndBeLateCount = 0;
            var totalTryCalssEffectiveCount = 0;
            var toatlMakeUpEffectiveCount = 0;
            foreach (var p in myClassTimes)
            {
                totalValidArrivedAndBeLateCount += p.ArrivedAndBeLateCount;
                totalTryCalssEffectiveCount += p.TryCalssEffectiveCount;
                toatlMakeUpEffectiveCount += p.MakeUpEffectiveCount;
            }
            if (_globalConfig.IncludeArrivedTryCalssStudent == EmBool.False)
            {
                totalValidArrivedAndBeLateCount -= totalTryCalssEffectiveCount;
            }
            if (_globalConfig.IncludeArrivedMakeUpStudent == EmBool.False)
            {
                totalValidArrivedAndBeLateCount -= toatlMakeUpEffectiveCount;
            }
            return totalValidArrivedAndBeLateCount;
        }

        /// <summary>
        /// 获取有效的到课人数
        /// </summary>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private int GetValidArrivedAndBeLateCount<T>(T myClassTimes) where T : BaseTeacherSalaryClassTimes
        {
            var totalValidArrivedAndBeLateCount = myClassTimes.ArrivedAndBeLateCount;
            if (_globalConfig.IncludeArrivedTryCalssStudent == EmBool.False)
            {
                totalValidArrivedAndBeLateCount -= myClassTimes.TryCalssStudentCount;
            }
            if (_globalConfig.IncludeArrivedMakeUpStudent == EmBool.False)
            {
                totalValidArrivedAndBeLateCount -= myClassTimes.MakeUpEffectiveCount;
            }
            return totalValidArrivedAndBeLateCount;
        }

        /// <summary>
        /// 无梯度_计算
        /// </summary>
        /// <param name="computeRelationValue">关联值</param>
        /// <param name="setRule">计算规则</param>
        /// <returns></returns>
        private decimal GetComputeSum_CalculateType_None(decimal computeRelationValue, EtTeacherSalaryContractPerformanceSetDetail setRule)
        {
            if (computeRelationValue <= 0)
            {
                return 0;
            }
            if (setRule.ComputeMode == EmTeacherSalaryComputeMode.StudentDeSum) //课消金额按比例绩效
            {
                if (setRule.ComputeValue <= 0)
                {
                    return 0;
                }
                else
                {
                    return computeRelationValue * setRule.ComputeValue / 100;
                }
            }
            else
            {
                return computeRelationValue * setRule.ComputeValue;
            }
        }

        /// <summary>
        /// 超额累计_计算
        /// </summary>
        /// <param name="computeRelationValue"></param>
        /// <param name="setRules"></param>
        /// <returns></returns>
        private decimal GetComputeSum_CalculateType_MoreThanValue(decimal surplusRelationValue, List<EtTeacherSalaryContractPerformanceSetDetail> setRules)
        {
            if (surplusRelationValue <= 0)
            {
                return 0;
            }
            var myRules = setRules.OrderBy(p => p.MinLimit);
            var totalMoney = 0M;
            if (setRules.First().ComputeMode == EmTeacherSalaryComputeMode.StudentDeSum)
            {
                foreach (var p in myRules)
                {
                    if (p.ComputeValue <= 0) //防止0作为除数
                    {
                        if (p.MaxLimit == null)
                        {
                            break;
                        }
                        var tempIntervalValue = p.MaxLimit.Value - p.MinLimit.Value;
                        surplusRelationValue -= tempIntervalValue;
                        if (surplusRelationValue <= 0)
                        {
                            break;
                        }
                    }

                    if (p.MaxLimit == null) //最后一项
                    {
                        totalMoney += surplusRelationValue * p.ComputeValue / 100;
                        break;
                    }
                    var intervalValue = p.MaxLimit.Value - p.MinLimit.Value;
                    if (surplusRelationValue > intervalValue)
                    {
                        totalMoney += intervalValue * p.ComputeValue / 100;
                        surplusRelationValue -= intervalValue;
                    }
                    else
                    {
                        totalMoney += surplusRelationValue * p.ComputeValue / 100;
                        break;
                    }
                }
            }
            else
            {
                foreach (var p in myRules)
                {
                    if (p.MaxLimit == null) //最后一项
                    {
                        totalMoney += surplusRelationValue * p.ComputeValue;
                        break;
                    }
                    var intervalValue = p.MaxLimit.Value - p.MinLimit.Value;
                    if (surplusRelationValue > intervalValue)
                    {
                        totalMoney += intervalValue * p.ComputeValue;
                        surplusRelationValue -= intervalValue;
                    }
                    else
                    {
                        totalMoney += surplusRelationValue * p.ComputeValue;
                        break;
                    }
                }
            }
            return totalMoney;
        }

        /// <summary>
        /// 全额累计_计算
        /// </summary>
        /// <param name="computeRelationValue"></param>
        /// <param name="setRules"></param>
        /// <returns></returns>
        private decimal GetComputeSum_CalculateType_AllValue(decimal computeRelationValue, List<EtTeacherSalaryContractPerformanceSetDetail> setRules)
        {
            if (computeRelationValue <= 0)
            {
                return 0;
            }
            var computeMode = setRules.First().ComputeMode;
            var myRules = setRules.OrderByDescending(p => p.MinLimit);
            foreach (var p in myRules)
            {
                if (computeRelationValue > p.MinLimit.Value)
                {
                    if (computeMode == EmTeacherSalaryComputeMode.StudentDeSum)
                    {
                        if (p.ComputeValue > 0)
                        {
                            return computeRelationValue * p.ComputeValue / 100;
                        }
                    }
                    else
                    {
                        return computeRelationValue * p.ComputeValue;
                    }
                    break;
                }
            }
            return 0;
        }

        private EtTeacherSalaryPayrollUserPerformanceDetail GetPerformanceDetail<T>(T myClassTimes, byte computeMode,
            byte computeType, decimal bascMoney, decimal itemComputeValue = 0) where T : BaseTeacherSalaryClassTimes
        {
            var courseId = 0L;
            if (myClassTimes is EtTeacherSalaryClassTimes)
            {
                courseId = (myClassTimes as EtTeacherSalaryClassTimes).CourseId;
            }
            return new EtTeacherSalaryPayrollUserPerformanceDetail()
            {
                ArrivedAndBeLateCount = myClassTimes.ArrivedAndBeLateCount,
                ArrivedCount = myClassTimes.ArrivedCount,
                BeLateCount = myClassTimes.BeLateCount,
                ClassId = myClassTimes.ClassId,
                ClassOt = myClassTimes.Ot,
                ClassRecordId = myClassTimes.ClassRecordId,
                ComputeMode = computeMode,
                ComputeType = computeType,
                CourseId = courseId,
                DeSum = myClassTimes.DeSum,
                EndTime = myClassTimes.EndTime,
                IsDeleted = myClassTimes.IsDeleted,
                LeaveCount = myClassTimes.LeaveCount,
                MakeUpEffectiveCount = myClassTimes.MakeUpEffectiveCount,
                MakeUpStudentCount = myClassTimes.MakeUpStudentCount,
                NotArrivedCount = myClassTimes.NotArrivedCount,
                StartTime = myClassTimes.StartTime,
                StudentClassTimes = myClassTimes.StudentClassTimes,
                TeacherClassTimes = myClassTimes.TeacherClassTimes,
                TenantId = myClassTimes.TenantId,
                UserId = _userId,
                Week = myClassTimes.Week,
                TryCalssEffectiveCount = myClassTimes.TryCalssEffectiveCount,
                TryCalssStudentCount = myClassTimes.TryCalssStudentCount,
                ComputeSum = itemComputeValue,
                BascMoney = bascMoney
            };
        }

        /// <summary>
        /// 按合计值_无梯度计算
        /// </summary>
        /// <returns></returns>
        private TeacherSalaryPayrollUserPerformanceView GetCalculateType_None<T>(long relationId, EtTeacherSalaryContractPerformanceSetDetail setRule,
            IEnumerable<T> myClassTimes, decimal bascMoney) where T : BaseTeacherSalaryClassTimes
        {
            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(setRule, bascMoney);
            var output = new EtTeacherSalaryPayrollUserPerformance()
            {
                ComputeMode = setRule.ComputeMode,
                ComputeType = setRule.ComputeType,
                RelationId = relationId,
                IsDeleted = EmIsDeleted.Normal,
                TenantId = _tenantId,
                UserId = _userId,
                ComputeDesc = $"<div class='performance_set_rule_title'>{desClassResult.Item2}</div>{desClassResult.Item1}"
            };
            switch (setRule.ComputeMode)
            {
                case EmTeacherSalaryComputeMode.TeacherClassTimes:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.TeacherClassTimes); 
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    output.ComputeRelationValue = GetValidArrivedAndBeLateCount(myClassTimes); 
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.DeSum); 
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.StudentClassTimes);
                    break;
            }
            if (output.ComputeRelationValue > 0)
            {
                output.ComputeSum = GetComputeSum_CalculateType_None(output.ComputeRelationValue, setRule);
            }
            if (bascMoney > 0)
            {
                output.ComputeSum += bascMoney * myClassTimes.Count();
            }

            output.SubmitSum = output.ComputeSum;

            var performanceDetails = new List<EtTeacherSalaryPayrollUserPerformanceDetail>();
            foreach (var p in myClassTimes)
            {
                performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney));
            }
            return new TeacherSalaryPayrollUserPerformanceView()
            {
                PerformanceDetails = performanceDetails,
                TeacherSalaryPayrollUserPerformance = output
            };
        }

        /// <summary>
        /// 按合计值_超额累计
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="setRules"></param>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private TeacherSalaryPayrollUserPerformanceView GetCalculateType_MoreThanValue<T>(long relationId, List<EtTeacherSalaryContractPerformanceSetDetail> setRules,
            IEnumerable<T> myClassTimes, decimal bascMoney) where T : BaseTeacherSalaryClassTimes
        {
            var firstRule = setRules.First();
            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(setRules, bascMoney);
            var output = new EtTeacherSalaryPayrollUserPerformance()
            {
                ComputeMode = firstRule.ComputeMode,
                ComputeType = firstRule.ComputeType,
                RelationId = relationId,
                IsDeleted = EmIsDeleted.Normal,
                TenantId = _tenantId,
                UserId = _userId,
                ComputeDesc = $"<div class='performance_set_rule_title'>{desClassResult.Item2}</div>{desClassResult.Item1}"
            };
            switch (firstRule.ComputeMode)
            {
                case EmTeacherSalaryComputeMode.TeacherClassTimes:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.TeacherClassTimes); 
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    output.ComputeRelationValue = GetValidArrivedAndBeLateCount(myClassTimes); 
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.DeSum); 
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.StudentClassTimes);
                    break;
            }
            if (output.ComputeRelationValue > 0)
            {
                output.ComputeSum = GetComputeSum_CalculateType_MoreThanValue(output.ComputeRelationValue, setRules);
            }
            if (bascMoney > 0)
            {
                output.ComputeSum += bascMoney * myClassTimes.Count();
            }
            output.SubmitSum = output.ComputeSum;

            var performanceDetails = new List<EtTeacherSalaryPayrollUserPerformanceDetail>();
            foreach (var p in myClassTimes)
            {
                performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney));
            }
            return new TeacherSalaryPayrollUserPerformanceView()
            {
                PerformanceDetails = performanceDetails,
                TeacherSalaryPayrollUserPerformance = output
            };
        }

        /// <summary>
        /// 按合计值_全额累计
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="setRules"></param>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private TeacherSalaryPayrollUserPerformanceView GetCalculateType_AllValue<T>(long relationId, List<EtTeacherSalaryContractPerformanceSetDetail> setRules,
            IEnumerable<T> myClassTimes, decimal bascMoney) where T : BaseTeacherSalaryClassTimes
        {
            var firstRule = setRules.First();
            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(setRules, bascMoney);
            var output = new EtTeacherSalaryPayrollUserPerformance()
            {
                ComputeMode = firstRule.ComputeMode,
                ComputeType = firstRule.ComputeType,
                RelationId = relationId,
                IsDeleted = EmIsDeleted.Normal,
                TenantId = _tenantId,
                UserId = _userId,
                ComputeDesc = $"<div class='performance_set_rule_title'>{desClassResult.Item2}</div>{desClassResult.Item1}"
            };
            switch (firstRule.ComputeMode)
            {
                case EmTeacherSalaryComputeMode.TeacherClassTimes:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.TeacherClassTimes); 
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    output.ComputeRelationValue = GetValidArrivedAndBeLateCount(myClassTimes); 
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.DeSum); 
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.StudentClassTimes);
                    break;
            }
            if (output.ComputeRelationValue > 0)
            {
                output.ComputeSum = GetComputeSum_CalculateType_AllValue(output.ComputeRelationValue, setRules);
            }
            if (bascMoney > 0)
            {
                output.ComputeSum += bascMoney * myClassTimes.Count();
            }
            output.SubmitSum = output.ComputeSum;

            var performanceDetails = new List<EtTeacherSalaryPayrollUserPerformanceDetail>();
            foreach (var p in myClassTimes)
            {
                performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney));
            }
            return new TeacherSalaryPayrollUserPerformanceView()
            {
                PerformanceDetails = performanceDetails,
                TeacherSalaryPayrollUserPerformance = output
            };
        }


        /// <summary>
        /// 按每个课次单独统计_无梯度计算
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="setRule"></param>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private TeacherSalaryPayrollUserPerformanceView GetCalculateType_None2<T>(long relationId, EtTeacherSalaryContractPerformanceSetDetail setRule,
            IEnumerable<T> myClassTimes, decimal bascMoney) where T : BaseTeacherSalaryClassTimes
        {
            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(setRule, bascMoney);
            var output = new EtTeacherSalaryPayrollUserPerformance()
            {
                ComputeMode = setRule.ComputeMode,
                ComputeType = setRule.ComputeType,
                RelationId = relationId,
                IsDeleted = EmIsDeleted.Normal,
                TenantId = _tenantId,
                UserId = _userId,
                ComputeDesc = $"<div class='performance_set_rule_title'>{desClassResult.Item2}</div>{desClassResult.Item1}"
            };
            var totalComputeSum = 0M;
            var totalComputeRelationValue = 0M;
            var performanceDetails = new List<EtTeacherSalaryPayrollUserPerformanceDetail>();
            decimal itemComputeValue;
            switch (setRule.ComputeMode)
            {
                case EmTeacherSalaryComputeMode.TeacherClassTimes:
                    foreach (var p in myClassTimes)
                    {
                        totalComputeRelationValue += p.TeacherClassTimes;
                        itemComputeValue = GetComputeSum_CalculateType_None(p.TeacherClassTimes, setRule);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    decimal validArrivedAndBeLateCount;
                    foreach (var p in myClassTimes)
                    {
                        validArrivedAndBeLateCount = GetValidArrivedAndBeLateCount(p);
                        totalComputeRelationValue += validArrivedAndBeLateCount;
                        itemComputeValue = GetComputeSum_CalculateType_None(validArrivedAndBeLateCount, setRule);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    foreach (var p in myClassTimes)
                    {
                        totalComputeRelationValue += p.DeSum;
                        itemComputeValue = GetComputeSum_CalculateType_None(p.DeSum, setRule);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    foreach (var p in myClassTimes)
                    {
                        totalComputeRelationValue += p.StudentClassTimes;
                        itemComputeValue = GetComputeSum_CalculateType_None(p.StudentClassTimes, setRule);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
            }
            output.ComputeRelationValue = totalComputeRelationValue;
            output.ComputeSum = totalComputeSum;
            if (bascMoney > 0)
            {
                output.ComputeSum += bascMoney * myClassTimes.Count();
            }
            output.SubmitSum = totalComputeSum;

            return new TeacherSalaryPayrollUserPerformanceView()
            {
                PerformanceDetails = performanceDetails,
                TeacherSalaryPayrollUserPerformance = output
            };
        }

        /// <summary>
        /// 按每个课次单独统计_超额累计
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="setRules"></param>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private TeacherSalaryPayrollUserPerformanceView GetCalculateType_MoreThanValue2<T>(long relationId, List<EtTeacherSalaryContractPerformanceSetDetail> setRules,
            IEnumerable<T> myClassTimes, decimal bascMoney) where T : BaseTeacherSalaryClassTimes
        {
            var firstRule = setRules.First();
            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(setRules, bascMoney);
            var output = new EtTeacherSalaryPayrollUserPerformance()
            {
                ComputeMode = firstRule.ComputeMode,
                ComputeType = firstRule.ComputeType,
                RelationId = relationId,
                IsDeleted = EmIsDeleted.Normal,
                TenantId = _tenantId,
                UserId = _userId,
                ComputeDesc = $"<div class='performance_set_rule_title'>{desClassResult.Item2}</div>{desClassResult.Item1}"
            };
            var totalComputeSum = 0M;
            var totalComputeRelationValue = 0M;
            var performanceDetails = new List<EtTeacherSalaryPayrollUserPerformanceDetail>();
            decimal itemComputeValue;
            switch (firstRule.ComputeMode)
            {
                case EmTeacherSalaryComputeMode.TeacherClassTimes:
                    foreach (var p in myClassTimes)
                    {
                        totalComputeRelationValue += p.TeacherClassTimes;
                        itemComputeValue = GetComputeSum_CalculateType_MoreThanValue(p.TeacherClassTimes, setRules);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    decimal validArrivedAndBeLateCount;
                    foreach (var p in myClassTimes)
                    {
                        validArrivedAndBeLateCount = GetValidArrivedAndBeLateCount(p);
                        totalComputeRelationValue += validArrivedAndBeLateCount;
                        itemComputeValue = GetComputeSum_CalculateType_MoreThanValue(validArrivedAndBeLateCount, setRules);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    foreach (var p in myClassTimes)
                    {
                        totalComputeRelationValue += p.DeSum;
                        itemComputeValue = GetComputeSum_CalculateType_MoreThanValue(p.DeSum, setRules);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    foreach (var p in myClassTimes)
                    {
                        totalComputeRelationValue += p.StudentClassTimes;
                        itemComputeValue = GetComputeSum_CalculateType_MoreThanValue(p.StudentClassTimes, setRules);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
            }
            output.ComputeRelationValue = totalComputeRelationValue;
            output.ComputeSum = totalComputeSum;
            if (bascMoney > 0)
            {
                output.ComputeSum += bascMoney * myClassTimes.Count();
            }
            output.SubmitSum = totalComputeSum;

            return new TeacherSalaryPayrollUserPerformanceView()
            {
                PerformanceDetails = performanceDetails,
                TeacherSalaryPayrollUserPerformance = output
            };
        }

        /// <summary>
        /// 按每个课次单独统计_全额累计
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="setRules"></param>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private TeacherSalaryPayrollUserPerformanceView GetCalculateType_AllValue2<T>(long relationId, List<EtTeacherSalaryContractPerformanceSetDetail> setRules,
            IEnumerable<T> myClassTimes, decimal bascMoney) where T : BaseTeacherSalaryClassTimes
        {
            var firstRule = setRules.First();
            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(setRules, bascMoney);
            var output = new EtTeacherSalaryPayrollUserPerformance()
            {
                ComputeMode = firstRule.ComputeMode,
                ComputeType = firstRule.ComputeType,
                RelationId = relationId,
                IsDeleted = EmIsDeleted.Normal,
                TenantId = _tenantId,
                UserId = _userId,
                ComputeDesc = $"<div class='performance_set_rule_title'>{desClassResult.Item2}</div>{desClassResult.Item1}"
            };
            var totalComputeSum = 0M;
            var totalComputeRelationValue = 0M;
            var performanceDetails = new List<EtTeacherSalaryPayrollUserPerformanceDetail>();
            decimal itemComputeValue;
            switch (firstRule.ComputeMode)
            {
                case EmTeacherSalaryComputeMode.TeacherClassTimes:
                    foreach (var p in myClassTimes)
                    {
                        totalComputeRelationValue += p.TeacherClassTimes;
                        itemComputeValue = GetComputeSum_CalculateType_AllValue(p.TeacherClassTimes, setRules);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    decimal validArrivedAndBeLateCount;
                    foreach (var p in myClassTimes)
                    {
                        validArrivedAndBeLateCount = GetValidArrivedAndBeLateCount(p);
                        totalComputeRelationValue += validArrivedAndBeLateCount;
                        itemComputeValue = GetComputeSum_CalculateType_AllValue(validArrivedAndBeLateCount, setRules);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    foreach (var p in myClassTimes)
                    {
                        totalComputeRelationValue += p.DeSum;
                        itemComputeValue = GetComputeSum_CalculateType_AllValue(p.DeSum, setRules);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    foreach (var p in myClassTimes)
                    {
                        totalComputeRelationValue += p.StudentClassTimes;
                        itemComputeValue = GetComputeSum_CalculateType_AllValue(p.StudentClassTimes, setRules);
                        totalComputeSum += itemComputeValue;
                        performanceDetails.Add(GetPerformanceDetail(p, output.ComputeMode, output.ComputeType, bascMoney, itemComputeValue));
                    }
                    break;
            }
            output.ComputeRelationValue = totalComputeRelationValue;
            output.ComputeSum = totalComputeSum;
            if (bascMoney > 0)
            {
                output.ComputeSum += bascMoney * myClassTimes.Count();
            }
            output.SubmitSum = totalComputeSum;

            return new TeacherSalaryPayrollUserPerformanceView()
            {
                PerformanceDetails = performanceDetails,
                TeacherSalaryPayrollUserPerformance = output
            };
        }

        #endregion

        /// <summary>
        /// 按合计值统计
        /// 按班级设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_0_0_0()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allClassIds = _mySalaryClassTimesList2.GroupBy(p => p.ClassId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myClassId in allClassIds)
            {
                var myClassSetRule = _myTeacherSalaryContractPerformanceSetDetails.FirstOrDefault(p => p.RelationId == myClassId);
                if (myClassSetRule == null)
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myClassId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList2.Where(p => p.ClassId == myClassId);
                output.Add(GetCalculateType_None(myClassId, myClassSetRule, myClassTimes, bascMoney));
            }
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 按班级设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_0_0_1()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allClassIds = _mySalaryClassTimesList2.GroupBy(p => p.ClassId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myClassId in allClassIds)
            {
                var myClassSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myClassId).ToList();
                if (myClassSetRules == null || !myClassSetRules.Any())
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myClassId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList2.Where(p => p.ClassId == myClassId);
                output.Add(GetCalculateType_MoreThanValue(myClassId, myClassSetRules, myClassTimes, bascMoney)); 
            }
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 按班级设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_0_0_2()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allClassIds = _mySalaryClassTimesList2.GroupBy(p => p.ClassId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myClassId in allClassIds)
            {
                var myClassSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myClassId).ToList();
                if (myClassSetRules == null || !myClassSetRules.Any())
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myClassId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList2.Where(p => p.ClassId == myClassId);
                output.Add(GetCalculateType_AllValue(myClassId, myClassSetRules, myClassTimes, bascMoney)); 
            }
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 按课程设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_0_1_0()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allCourseIds = _mySalaryClassTimesList.GroupBy(p => p.CourseId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myCourseId in allCourseIds)
            {
                var myCourseSetRule = _myTeacherSalaryContractPerformanceSetDetails.FirstOrDefault(p => p.RelationId == myCourseId);
                if (myCourseSetRule == null)
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myCourseId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList.Where(p => p.CourseId == myCourseId);
                output.Add(GetCalculateType_None(myCourseId, myCourseSetRule, myClassTimes, bascMoney));
            }
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 按课程设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_0_1_1()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allCourseIds = _mySalaryClassTimesList.GroupBy(p => p.CourseId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myCourseId in allCourseIds)
            {
                var myCourseSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myCourseId).ToList();
                if (myCourseSetRules == null || !myCourseSetRules.Any())
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myCourseId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList.Where(p => p.CourseId == myCourseId);
                output.Add(GetCalculateType_MoreThanValue(myCourseId, myCourseSetRules, myClassTimes, bascMoney)); 
            }
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 按课程设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_0_1_2()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allCourseIds = _mySalaryClassTimesList.GroupBy(p => p.CourseId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myCourseId in allCourseIds)
            {
                var myCourseSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myCourseId).ToList();
                if (myCourseSetRules == null || !myCourseSetRules.Any())
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myCourseId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList.Where(p => p.CourseId == myCourseId);
                output.Add(GetCalculateType_AllValue(myCourseId, myCourseSetRules, myClassTimes, myCourseId)); 
            }
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 统一设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_0_2_0()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var bascMoney = 0M;
            if (_myTeacherSalaryContractPerformanceLessonBascs != null)
            {
                var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == 0);
                if (myBascMoneySet != null)
                {
                    bascMoney = myBascMoneySet.ComputeValue;
                }
            }
            output.Add(GetCalculateType_None(0, _myTeacherSalaryContractPerformanceSetDetails.First(), _mySalaryClassTimesList2, bascMoney));
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 统一设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_0_2_1()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var bascMoney = 0M;
            if (_myTeacherSalaryContractPerformanceLessonBascs != null)
            {
                var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == 0);
                if (myBascMoneySet != null)
                {
                    bascMoney = myBascMoneySet.ComputeValue;
                }
            }
            output.Add(GetCalculateType_MoreThanValue(0, _myTeacherSalaryContractPerformanceSetDetails, _mySalaryClassTimesList2, bascMoney));
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 统一设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_0_2_2()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var bascMoney = 0M;
            if (_myTeacherSalaryContractPerformanceLessonBascs != null)
            {
                var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == 0);
                if (myBascMoneySet != null)
                {
                    bascMoney = myBascMoneySet.ComputeValue;
                }
            }
            output.Add(GetCalculateType_AllValue(0, _myTeacherSalaryContractPerformanceSetDetails, _mySalaryClassTimesList2, bascMoney));
            return output;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按班级设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_1_0_0()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allClassIds = _mySalaryClassTimesList2.GroupBy(p => p.ClassId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myClassId in allClassIds)
            {
                var myClassSetRule = _myTeacherSalaryContractPerformanceSetDetails.FirstOrDefault(p => p.RelationId == myClassId);
                if (myClassSetRule == null)
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myClassId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList2.Where(p => p.ClassId == myClassId);
                output.Add(GetCalculateType_None2(myClassId, myClassSetRule, myClassTimes, bascMoney));
            }
            return output;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按班级设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_1_0_1()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allClassIds = _mySalaryClassTimesList2.GroupBy(p => p.ClassId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myClassId in allClassIds)
            {
                var myClassSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myClassId).ToList();
                if (myClassSetRules == null || !myClassSetRules.Any())
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myClassId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList2.Where(p => p.ClassId == myClassId);
                output.Add(GetCalculateType_MoreThanValue2(myClassId, myClassSetRules, myClassTimes, bascMoney)); 
            }
            return output;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按班级设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_1_0_2()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allClassIds = _mySalaryClassTimesList2.GroupBy(p => p.ClassId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myClassId in allClassIds)
            {
                var myClassSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myClassId).ToList();
                if (myClassSetRules == null || !myClassSetRules.Any())
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myClassId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList2.Where(p => p.ClassId == myClassId);
                output.Add(GetCalculateType_AllValue2(myClassId, myClassSetRules, myClassTimes, bascMoney)); 
            }
            return output;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按课程设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_1_1_0()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allCourseIds = _mySalaryClassTimesList.GroupBy(p => p.CourseId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myCourseId in allCourseIds)
            {
                var myCourseSetRule = _myTeacherSalaryContractPerformanceSetDetails.FirstOrDefault(p => p.RelationId == myCourseId);
                if (myCourseSetRule == null)
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myCourseId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList.Where(p => p.CourseId == myCourseId);
                output.Add(GetCalculateType_None2(myCourseId, myCourseSetRule, myClassTimes, bascMoney));
            }
            return output;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按课程设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_1_1_1()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allCourseIds = _mySalaryClassTimesList.GroupBy(p => p.CourseId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myCourseId in allCourseIds)
            {
                var myCourseSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myCourseId).ToList();
                if (myCourseSetRules == null || !myCourseSetRules.Any())
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myCourseId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList.Where(p => p.CourseId == myCourseId);
                output.Add(GetCalculateType_MoreThanValue2(myCourseId, myCourseSetRules, myClassTimes, bascMoney)); 
            }
            return output;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按课程设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_1_1_2()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var allCourseIds = _mySalaryClassTimesList.GroupBy(p => p.CourseId).Select(p => p.Key);
            var bascMoney = 0M;
            foreach (var myCourseId in allCourseIds)
            {
                var myCourseSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myCourseId).ToList();
                if (myCourseSetRules == null || !myCourseSetRules.Any())
                {
                    continue;
                }

                bascMoney = 0;
                if (_myTeacherSalaryContractPerformanceLessonBascs != null)
                {
                    var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == myCourseId);
                    if (myBascMoneySet != null)
                    {
                        bascMoney = myBascMoneySet.ComputeValue;
                    }
                }

                var myClassTimes = _mySalaryClassTimesList.Where(p => p.CourseId == myCourseId);
                output.Add(GetCalculateType_AllValue2(myCourseId, myCourseSetRules, myClassTimes, bascMoney)); 
            }
            return output;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 统一设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_1_2_0()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var bascMoney = 0M;
            if (_myTeacherSalaryContractPerformanceLessonBascs != null)
            {
                var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == 0);
                if (myBascMoneySet != null)
                {
                    bascMoney = myBascMoneySet.ComputeValue;
                }
            }
            output.Add(GetCalculateType_None2(0, _myTeacherSalaryContractPerformanceSetDetails.First(), _mySalaryClassTimesList2, bascMoney));
            return output;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 统一设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_1_2_1()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var bascMoney = 0M;
            if (_myTeacherSalaryContractPerformanceLessonBascs != null)
            {
                var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == 0);
                if (myBascMoneySet != null)
                {
                    bascMoney = myBascMoneySet.ComputeValue;
                }
            }
            output.Add(GetCalculateType_MoreThanValue2(0, _myTeacherSalaryContractPerformanceSetDetails, _mySalaryClassTimesList2, bascMoney));
            return output;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 统一设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<TeacherSalaryPayrollUserPerformanceView> Process_1_2_2()
        {
            var output = new List<TeacherSalaryPayrollUserPerformanceView>();
            var bascMoney = 0M;
            if (_myTeacherSalaryContractPerformanceLessonBascs != null)
            {
                var myBascMoneySet = _myTeacherSalaryContractPerformanceLessonBascs.FirstOrDefault(p => p.RelationId == 0);
                if (myBascMoneySet != null)
                {
                    bascMoney = myBascMoneySet.ComputeValue;
                }
            }
            output.Add(GetCalculateType_AllValue2(0, _myTeacherSalaryContractPerformanceSetDetails, _mySalaryClassTimesList2, bascMoney));
            return output;
        }
    }
}
