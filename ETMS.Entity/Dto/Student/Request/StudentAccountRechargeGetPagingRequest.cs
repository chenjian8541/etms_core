using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentAccountRechargeGetPagingRequest : RequestPagingBase
    {
        public string Phone { get; set; }

        public decimal? BalanceSumMin { get; set; }

        public decimal? BalanceSumMax { get; set; }

        public decimal? BalanceRealMin { get; set; }

        public decimal? BalanceRealMax { get; set; }

        public decimal? BalanceGiveMin { get; set; }

        public decimal? BalanceGiveMax { get; set; }

        public decimal? RechargeSumMin { get; set; }

        public decimal? RechargeSumMax { get; set; }

        public decimal? RechargeGiveSumMin { get; set; }

        public decimal? RechargeGiveSumMax { get; set; }

        public long? StudentId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Phone))
            {
                condition.Append($" AND Phone = '{Phone}'");
            }
            if (BalanceSumMin != null)
            {
                condition.Append($" AND BalanceSum >= {BalanceSumMin}");
            }
            if (BalanceSumMax != null)
            {
                condition.Append($" AND BalanceSum <= {BalanceSumMax}");
            }
            if (BalanceRealMin != null)
            {
                condition.Append($" AND BalanceReal >= {BalanceRealMin}");
            }
            if (BalanceRealMax != null)
            {
                condition.Append($" AND BalanceReal <= {BalanceRealMax}");
            }
            if (BalanceGiveMin != null)
            {
                condition.Append($" AND BalanceGive >= {BalanceGiveMin}");
            }
            if (BalanceGiveMax != null)
            {
                condition.Append($" AND BalanceGive <= {BalanceGiveMax}");
            }
            if (RechargeSumMin != null)
            {
                condition.Append($" AND RechargeSum >= {RechargeSumMin}");
            }
            if (RechargeSumMax != null)
            {
                condition.Append($" AND RechargeSum <= {RechargeSumMax}");
            }
            if (RechargeGiveSumMin != null)
            {
                condition.Append($" AND RechargeGiveSum >= {RechargeGiveSumMin}");
            }
            if (RechargeGiveSumMax != null)
            {
                condition.Append($" AND RechargeGiveSum <= {RechargeGiveSumMax}");
            }
            if (StudentId != null)
            {
                condition.Append($" AND RelationStudentIds LIKE '%,{StudentId},%'");
            }
            return condition.ToString();
        }

        public override string Validate()
        {
            if (BalanceSumMin != null && BalanceSumMax != null && BalanceSumMin > BalanceSumMax)
            {
                return "总余额 最大值必须大于最小值";
            }
            if (BalanceRealMin != null && BalanceRealMax != null && BalanceRealMin > BalanceRealMax)
            {
                return "实充余额 最大值必须大于最小值";
            }
            if (BalanceGiveMin != null && BalanceGiveMax != null && BalanceGiveMin > BalanceGiveMax)
            {
                return "赠送余额 最大值必须大于最小值";
            }
            if (RechargeSumMin != null && RechargeSumMax != null && RechargeSumMin > RechargeSumMax)
            {
                return "累计实充金额 最大值必须大于最小值";
            }
            if (RechargeGiveSumMin != null && RechargeGiveSumMax != null && RechargeGiveSumMin > RechargeGiveSumMax)
            {
                return "累计赠送金额 最大值必须大于最小值";
            }
            return base.Validate();
        }
    }
}