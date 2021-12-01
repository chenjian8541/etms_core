using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETMS.Entity.Dto.HisData.Request
{
    public class OrderGetDetailPagingRequest : RequestPagingBase, IDataLimit
    {
        public long? StudentId { get; set; }

        public string No { get; set; }

        public long? UserId { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        public byte? Status { get; set; }

        /// <summary>
        /// 订单状态  0:正常  否则代表已作废
        /// </summary>
        public byte? OrderStatus { get; set; }

        public byte? OrderType { get; set; }

        public byte? BuyType { get; set; }

        public byte? InOutType { get; set; }

        public byte? ProductType { get; set; }

        public List<long> ProductInfo { get; set; }

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
                condition.Append($" AND StudentId = {StudentId.Value}");
            }
            if (!string.IsNullOrEmpty(No))
            {
                condition.Append($" AND OrderNo LIKE '%{No}'");
            }
            if (UserId != null)
            {
                condition.Append($" AND UserId = {UserId.Value}");
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
            if (BuyType != null)
            {
                condition.Append($" AND [BuyType] = {BuyType.Value}");
            }
            if (InOutType != null)
            {
                condition.Append($" AND [InOutType] = {InOutType.Value}");
            }
            if (ProductType != null)
            {
                condition.Append($" AND [ProductType] = {ProductType.Value}");
            }
            if (ProductInfo != null && ProductInfo.Count == 2)
            {
                condition.Append($" AND [ProductType] = {ProductInfo[0]} AND ProductId = {ProductInfo[1]}");
            }
            if (StartOt != null)
            {
                condition.Append($" AND Ot >= '{StartOt.Value.EtmsToString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND Ot < '{EndOt.Value.EtmsToString()}'");
            }
            if (IsDataLimit)
            {
                condition.Append(GetDataLimitFilterWhere());
            }
            return condition.ToString();
        }
    }
}
