using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveHomeworkGetPagingRequest : RequestPagingBase, IDataLimit
    {
        public string Title { get; set; }

        public long? TeacherId { get; set; }

        public long? ClassId { get; set; }

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
            return $" AND CreateUserId = {LoginUserId}";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Title))
            {
                condition.Append($" AND Title LIKE '{Title}%'");
            }
            if (TeacherId != null)
            {
                condition.Append($" AND CreateUserId = {TeacherId.Value}");
            }
            if (ClassId != null)
            {
                condition.Append($" AND ClassId = {ClassId.Value}");
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
