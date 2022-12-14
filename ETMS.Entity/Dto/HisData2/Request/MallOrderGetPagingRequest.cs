using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.HisData2.Request
{
    public class MallOrderGetPagingRequest : RequestPagingBase
    {
        public string GoodsName { get; set; }

        public long? StudentId { get; set; }

        public string No { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        public byte? Status { get; set; }

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

        public byte? ProductType { get; set; }

        public string Remark { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (StudentId != null)
            {
                condition.Append($" AND StudentId = {StudentId.Value}");
            }
            if (!string.IsNullOrEmpty(GoodsName))
            {
                condition.Append($" AND GoodsName LIKE '%{GoodsName}%'");
            }
            if (!string.IsNullOrEmpty(No))
            {
                condition.Append($" AND OrderNo = '{No}'");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value}");
            }
            if (StartOt != null)
            {
                condition.Append($" AND CreateOt >= '{StartOt.Value.EtmsToString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND CreateOt < '{EndOt.Value.EtmsToString()}'");
            }
            if (ProductType != null)
            {
                condition.Append($" AND [ProductType] = {ProductType.Value}");
            }
            if (!string.IsNullOrEmpty(Remark))
            {
                condition.Append($" AND Remark LIKE '%{Remark}%'");
            }
            return condition.ToString();
        }
    }
}
