using ETMS.Business.Common;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Enum;
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
        private IEnumerable<EtTeacherSalaryClassTimes> _mySalaryClassTimesList;
        private TeacherSalaryGlobalRuleView _globalConfig;
        public UserSalaryPerformanceHandler(int tenantId, long userId, EtTeacherSalaryContractPerformanceSet myTeacherSalaryContractPerformanceSet,
            List<EtTeacherSalaryContractPerformanceSetDetail> myTeacherSalaryContractPerformanceSetDetails,
            IEnumerable<EtTeacherSalaryClassTimes> mySalaryClassTimesList, TeacherSalaryGlobalRuleView teacherSalaryGlobalRuleView)
        {
            this._tenantId = tenantId;
            this._userId = userId;
            this._myTeacherSalaryContractPerformanceSet = myTeacherSalaryContractPerformanceSet;
            this._myTeacherSalaryContractPerformanceSetDetails = myTeacherSalaryContractPerformanceSetDetails;
            this._mySalaryClassTimesList = mySalaryClassTimesList;
            this._globalConfig = teacherSalaryGlobalRuleView;
        }

        /// <summary>
        /// 获取有效的到课人数
        /// </summary>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private int GetValidArrivedAndBeLateCount(IEnumerable<EtTeacherSalaryClassTimes> myClassTimes)
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
        /// 合计值_无梯度计算
        /// </summary>
        /// <returns></returns>
        private EtTeacherSalaryPayrollUserPerformance GetCalculateType_None(long relationId, EtTeacherSalaryContractPerformanceSetDetail setRule,
            IEnumerable<EtTeacherSalaryClassTimes> myClassTimes)
        {
            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(setRule);
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
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.TeacherClassTimes); ;
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    output.ComputeRelationValue = GetValidArrivedAndBeLateCount(myClassTimes); ;
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.DeSum); ;
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.StudentClassTimes);
                    break;
            }
            if (output.ComputeRelationValue > 0)
            {
                if (setRule.ComputeMode == EmTeacherSalaryComputeMode.StudentDeSum) //课消金额按比例绩效
                {
                    if (setRule.ComputeValue <= 0)
                    {
                        output.ComputeSum = 0;
                    }
                    else
                    {
                        output.ComputeSum = output.ComputeRelationValue * setRule.ComputeValue / 100;
                    }
                }
                else
                {
                    output.ComputeSum = output.ComputeRelationValue * setRule.ComputeValue;
                }
            }
            output.SubmitSum = output.ComputeSum;
            return output;
        }

        /// <summary>
        /// 合计值_超额累计
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="setRules"></param>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private EtTeacherSalaryPayrollUserPerformance GetCalculateType_MoreThanValue(long relationId, List<EtTeacherSalaryContractPerformanceSetDetail> setRules,
            IEnumerable<EtTeacherSalaryClassTimes> myClassTimes)
        {
            var firstRule = setRules.First();
            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(setRules);
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
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.TeacherClassTimes); ;
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    output.ComputeRelationValue = GetValidArrivedAndBeLateCount(myClassTimes); ;
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.DeSum); ;
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.StudentClassTimes);
                    break;
            }
            if (output.ComputeRelationValue > 0)
            {
                var myRules = setRules.OrderBy(p => p.MinLimit);
                var totalMoney = 0M;
                var surplusRelationValue = output.ComputeRelationValue;
                if (firstRule.ComputeMode == EmTeacherSalaryComputeMode.StudentDeSum)
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
                output.ComputeSum = totalMoney;
            }
            output.SubmitSum = output.ComputeSum;
            return output;
        }

        /// <summary>
        /// 合计值_全额累计
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="setRules"></param>
        /// <param name="myClassTimes"></param>
        /// <returns></returns>
        private EtTeacherSalaryPayrollUserPerformance GetCalculateType_AllValue(long relationId, List<EtTeacherSalaryContractPerformanceSetDetail> setRules,
            IEnumerable<EtTeacherSalaryClassTimes> myClassTimes)
        {
            var firstRule = setRules.First();
            var desClassResult = ComBusiness4.GetTeacherSalaryContractPerformanceSetDetailDesc(setRules);
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
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.TeacherClassTimes); ;
                    break;
                case EmTeacherSalaryComputeMode.StudentAttendeesCount:
                    output.ComputeRelationValue = GetValidArrivedAndBeLateCount(myClassTimes); ;
                    break;
                case EmTeacherSalaryComputeMode.StudentDeSum:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.DeSum); ;
                    break;
                case EmTeacherSalaryComputeMode.StudentClassTimes:
                    output.ComputeRelationValue = myClassTimes.Sum(p => p.StudentClassTimes);
                    break;
            }
            if (output.ComputeRelationValue > 0)
            {
                var myRules = setRules.OrderByDescending(p => p.MinLimit);
                foreach (var p in myRules)
                {
                    if (output.ComputeRelationValue > p.MinLimit.Value)
                    {
                        if (firstRule.ComputeMode == EmTeacherSalaryComputeMode.StudentDeSum)
                        {
                            if (p.ComputeValue > 0)
                            {
                                output.ComputeSum = output.ComputeRelationValue * p.ComputeValue / 100;
                            }
                        }
                        else
                        {
                            output.ComputeSum = output.ComputeRelationValue * p.ComputeValue;
                        }
                        break;
                    }
                }
            }
            output.SubmitSum = output.ComputeSum;
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 按班级设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_0_0_0()
        {
            var output = new List<EtTeacherSalaryPayrollUserPerformance>();
            var allClassIds = _mySalaryClassTimesList.GroupBy(p => p.ClassId).Select(p => p.Key);
            foreach (var myClassId in allClassIds)
            {
                var myClassSetRule = _myTeacherSalaryContractPerformanceSetDetails.FirstOrDefault(p => p.RelationId == myClassId);
                if (myClassSetRule == null)
                {
                    continue;
                }
                var myClassTimes = _mySalaryClassTimesList.Where(p => p.ClassId == myClassId);
                output.Add(GetCalculateType_None(myClassId, myClassSetRule, myClassTimes));
            }
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 按班级设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_0_0_1()
        {
            var output = new List<EtTeacherSalaryPayrollUserPerformance>();
            var allClassIds = _mySalaryClassTimesList.GroupBy(p => p.ClassId).Select(p => p.Key);
            foreach (var myClassId in allClassIds)
            {
                var myClassSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myClassId).ToList();
                if (myClassSetRules == null || !myClassSetRules.Any())
                {
                    continue;
                }
                var myClassTimes = _mySalaryClassTimesList.Where(p => p.ClassId == myClassId);
                output.Add(GetCalculateType_MoreThanValue(myClassId, myClassSetRules, myClassTimes)); ;
            }
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 按班级设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_0_0_2()
        {
            var output = new List<EtTeacherSalaryPayrollUserPerformance>();
            var allClassIds = _mySalaryClassTimesList.GroupBy(p => p.ClassId).Select(p => p.Key);
            foreach (var myClassId in allClassIds)
            {
                var myClassSetRules = _myTeacherSalaryContractPerformanceSetDetails.Where(p => p.RelationId == myClassId).ToList();
                if (myClassSetRules == null || !myClassSetRules.Any())
                {
                    continue;
                }
                var myClassTimes = _mySalaryClassTimesList.Where(p => p.ClassId == myClassId);
                output.Add(GetCalculateType_AllValue(myClassId, myClassSetRules, myClassTimes)); ;
            }
            return output;
        }

        /// <summary>
        /// 按合计值统计
        /// 按课程设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_0_1_0()
        {
            return null;
        }

        /// <summary>
        /// 按合计值统计
        /// 按课程设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_0_1_1()
        {
            return null;
        }

        /// <summary>
        /// 按合计值统计
        /// 按课程设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_0_1_2()
        {
            return null;
        }

        /// <summary>
        /// 按合计值统计
        /// 统一设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_0_2_0()
        {
            return null;
        }

        /// <summary>
        /// 按合计值统计
        /// 统一设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_0_2_1()
        {
            return null;
        }

        /// <summary>
        /// 按合计值统计
        /// 统一设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_0_2_2()
        {
            return null;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按班级设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_1_0_0()
        {
            return null;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按班级设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_1_0_1()
        {
            return null;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按班级设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_1_0_2()
        {
            return null;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按课程设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_1_1_0()
        {
            return null;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按课程设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_1_1_1()
        {
            return null;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 按课程设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_1_1_2()
        {
            return null;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 统一设置
        /// 无梯度
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_1_2_0()
        {
            return null;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 统一设置
        /// 超额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_1_2_1()
        {
            return null;
        }

        /// <summary>
        /// 按每个课次单独统计
        /// 统一设置
        /// 全额累计
        /// </summary>
        /// <returns></returns>
        public List<EtTeacherSalaryPayrollUserPerformance> Process_1_2_2()
        {
            return null;
        }
    }
}
