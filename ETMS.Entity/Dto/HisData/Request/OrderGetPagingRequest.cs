using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class OrderGetPagingRequest : RequestPagingBase, IDataLimit
    {
        public long? StudentId { get; set; }

        public long? StudentAccountRechargeId { get; set; }

        public string No { get; set; }

        public long? UserId { get; set; }

        public long? CommissionUser { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 订单状态  0:正常  否则代表已作废
        /// </summary>
        public byte? OrderStatus { get; set; }

        public byte? OrderType { get; set; }

        public int? OrderSource { get; set; }


        /// <summary>
        /// 查询时间
        /// </summary>
        public List<string> Ot { get; set; }

        private DateTime? _startOt;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartOt
        {
            get
            {
                if (_startOt != null)
                {
                    return _startOt;
                }
                if (Ot == null || Ot.Count == 0)
                {
                    return null;
                }
                _startOt = Convert.ToDateTime(Ot[0]);
                return _startOt;
            }
        }

        private DateTime? _endOt;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndOt
        {
            get
            {
                if (_endOt != null)
                {
                    return _endOt;
                }
                if (Ot == null || Ot.Count < 2)
                {
                    return null;
                }
                _endOt = Convert.ToDateTime(Ot[1]).AddDays(1); ;
                return _endOt;
            }
        }

        public bool? IsQueryHasArrears { get; set; }

        public string Remark { get; set; }

        public string GetDataLimitFilterWhere()
        {
            return $" AND UserId = {LoginUserId}";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (StudentId != null)
            {
                if (StudentAccountRechargeId != null)
                {
                    condition.Append($" AND (StudentId = {StudentId.Value} OR StudentAccountRechargeId = {StudentAccountRechargeId.Value})");
                }
                else
                {
                    condition.Append($" AND StudentId = {StudentId.Value}");
                }
            }
            if (!string.IsNullOrEmpty(No))
            {
                condition.Append($" AND No LIKE '%{No}'");
            }
            if (UserId != null)
            {
                condition.Append($" AND UserId = {UserId.Value}");
            }
            if (CommissionUser != null)
            {
                condition.Append($" AND CommissionUser LIKE '%,{CommissionUser.Value},%'");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value}");
            }
            if (OrderStatus != null)
            {
                if (OrderStatus == 0)
                {
                    condition.Append($" AND [Status] <> {EmOrderStatus.Repeal}");
                }
                else
                {
                    condition.Append($" AND [Status] = {EmOrderStatus.Repeal}");
                }
            }
            if (OrderType != null)
            {
                condition.Append($" AND [OrderType] = {OrderType.Value}");
            }
            if (OrderSource != null)
            {
                condition.Append($" AND [OrderSource] = {OrderSource.Value}");
            }
            if (StartOt != null)
            {
                condition.Append($" AND Ot >= '{StartOt.Value.EtmsToString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND Ot < '{EndOt.Value.EtmsToString()}'");
            }
            if (!string.IsNullOrEmpty(Remark))
            {
                condition.Append($" AND Remark LIKE '%{Remark}%'");
            }
            if (IsQueryHasArrears != null && IsQueryHasArrears.Value)
            {
                condition.Append($" AND ArrearsSum > 0 AND [Status] <> {EmOrderStatus.Repeal}");
            }
            if (IsDataLimit)
            {
                condition.Append(GetDataLimitFilterWhere());
            }
            return condition.ToString();
        }
    }
}
