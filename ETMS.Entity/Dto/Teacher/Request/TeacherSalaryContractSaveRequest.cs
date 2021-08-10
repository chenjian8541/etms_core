using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Teacher.Request
{
    public class TeacherSalaryContractSaveRequest : RequestBase
    {
        public long TeacherId { get; set; }

        public List<ContractFixedItem> ContractFixeds { get; set; }

        public ContractPerformanceSet ContractPerformanceSet { get; set; }

        public List<PerformanceSetDetail> PerformanceSetDetails { get; set; }

        public override string Validate()
        {
            if (TeacherId <= 0)
            {
                return "请求数据格式错误";
            }
            if (ContractFixeds == null || ContractFixeds.Count == 0)
            {
                return "请设置固定工资";
            }
            var errMsg = string.Empty;
            foreach (var p in ContractFixeds)
            {
                errMsg = p.Validate();
                if (!string.IsNullOrEmpty(errMsg))
                {
                    return errMsg;
                }
            }

            if (PerformanceSetDetails != null && PerformanceSetDetails.Count > 0)
            {
                foreach (var p in PerformanceSetDetails)
                {
                    errMsg = p.Validate();
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        return errMsg;
                    }
                }
            }
            return string.Empty;
        }
    }

    public class ContractFixedItem : IValidate
    {
        public long FundsItemsId { get; set; }

        public decimal AmountValue { get; set; }

        public string Validate()
        {
            if (FundsItemsId <= 0)
            {
                return "固定工资-请求数据格式错误";
            }
            return string.Empty;
        }
    }

    public class ContractPerformanceSet
    {
        public byte ComputeType { get; set; }

        public byte GradientCalculateType { get; set; }
    }

    public class PerformanceSetDetail : IValidate
    {
        /// <summary>
        /// 关联ID (班级、课程)
        /// 0：代表全局设置
        /// </summary>
        public long RelationId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmTeacherSalaryComputeMode"/>
        /// </summary>
        public byte ComputeMode { get; set; }

        public decimal? MinLimit { get; set; }

        public decimal? MaxLimit { get; set; }

        public decimal ComputeValue { get; set; }

        public string Validate()
        {
            if (MinLimit != null && MaxLimit != null)
            {
                if (MinLimit >= MaxLimit)
                {
                    return "梯度区间设置无效";
                }
            }
            if (ComputeValue <= 0)
            {
                return "绩效计算值必须大于0";
            }
            return string.Empty;
        }
    }
}
