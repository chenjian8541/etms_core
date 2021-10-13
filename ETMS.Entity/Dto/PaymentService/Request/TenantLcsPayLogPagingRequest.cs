using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Request
{
    public class TenantLcsPayLogPagingRequest : RequestPagingBase
    {
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

        public int? OrderType { get; set; }

        public int? OrderSource { get; set; }

        public long? StudentId { get; set; }

        public string OrderNo { get; set; }

        public string OutTradeNo { get; set; }

        public int? Status { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (StudentId != null)
            {
                condition.Append($" AND RelationId = {StudentId.Value}");
            }
            if (StartOt != null)
            {
                condition.Append($" AND PayFinishOt >= '{StartOt.Value.EtmsToDateString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND PayFinishOt < '{EndOt.Value.EtmsToDateString()}'");
            }
            if (OrderType != null)
            {
                condition.Append($" AND OrderType = {OrderType.Value}");
            }
            if (!string.IsNullOrEmpty(OrderNo))
            {
                condition.Append($" AND OrderNo LIKE '%{OrderNo}'");
            }
            if (!string.IsNullOrEmpty(OutTradeNo))
            {
                condition.Append($" AND OutTradeNo LIKE '%{OutTradeNo}'");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value}");
            }
            if (OrderSource != null)
            {
                condition.Append($" AND [OrderSource] = {OrderSource.Value}");
            }
            return condition.ToString();
        }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (StartOt != null && EndOt != null && StartOt > EndOt)
            {
                return "开始时间不能大于结束时间";
            }
            return base.Validate();
        }
    }
}
