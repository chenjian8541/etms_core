using ETMS.Entity.EtmsManage.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Agent.Request
{
    public class AgentEtmsAccountLogPagingRequest : AgentPagingBase
    {
        public int? AgentId { get; set; }

        /// <summary>
        /// 代理商信息
        /// </summary>
        public string Key { get; set; }

        public string Remark { get; set; }

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

        public byte? ChangeType { get; set; }

        public long? UserId { get; set; }

        /// <summary>
        /// 是否需要限制用户数据
        /// </summary>
        /// <returns></returns>
        public override bool IsNeedLimitUserData()
        {
            return true;
        }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet());
            if (!string.IsNullOrEmpty(Key))
            {
                condition.Append($" AND (AgentName LIKE '{Key}%' OR AgentPhone LIKE '{Key}%')");
            }
            if (StartOt != null)
            {
                condition.Append($" AND Ot >= '{StartOt.Value.EtmsToDateString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND Ot < '{EndOt.Value.EtmsToDateString()}'");
            }
            if (!string.IsNullOrEmpty(Remark))
            {
                condition.Append($" AND Remark LIKE '%{Remark}%'");
            }
            if (AgentId != null)
            {
                condition.Append($" AND AgentId = {AgentId.Value}");
            }
            if (ChangeType != null)
            {
                condition.Append($" AND [ChangeType] = {ChangeType.Value}");
            }
            if (UserId != null)
            {
                condition.Append($" AND UserId = {UserId.Value}");
            }
            return condition.ToString();
        }
    }
}
