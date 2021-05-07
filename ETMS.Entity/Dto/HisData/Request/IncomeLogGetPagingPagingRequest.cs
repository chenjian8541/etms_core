using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class IncomeLogGetPagingPagingRequest : RequestPagingBase, IDataLimit
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

        public int? ProjectType { get; set; }

        public int? Type { get; set; }

        public string AccountNo { get; set; }

        public string No { get; set; }

        public long? UserId { get; set; }

        public int? PayType { get; set; }

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
            if (StartOt != null)
            {
                condition.Append($" AND Ot >= '{StartOt.Value.EtmsToString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND Ot < '{EndOt.Value.EtmsToString()}'");
            }
            if (ProjectType != null)
            {
                condition.Append($" AND ProjectType = {ProjectType.Value}");
            }
            if (Type != null)
            {
                condition.Append($" AND [Type] = {Type.Value}");
            }
            if (!string.IsNullOrEmpty(AccountNo))
            {
                condition.Append($" AND AccountNo = '{AccountNo}'");
            }
            if (!string.IsNullOrEmpty(No))
            {
                condition.Append($" AND No = '{No}'");
            }
            if (UserId != null)
            {
                condition.Append($" AND UserId = {UserId.Value}");
            }
            if (PayType != null)
            {
                condition.Append($" AND PayType = {PayType.Value}");
            }
            if (!string.IsNullOrEmpty(Remark))
            {
                condition.Append($" AND Remark LIKE '%{Remark}%'");
            }
            if (IsDataLimit)
            {
                condition.Append(GetDataLimitFilterWhere());
            }
            return condition.ToString();
        }
    }
}
