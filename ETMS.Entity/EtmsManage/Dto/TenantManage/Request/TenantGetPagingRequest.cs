using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.EtmsManage.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.TenantManage.Request
{
    public class TenantGetPagingRequest : AgentPagingBase
    {
        /// <summary>
        /// 代理商信息
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 机构信息
        /// </summary>
        public string TenantKey { get; set; }

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

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmSysTenantStatus"/>
        /// </summary>
        public byte? Status { get; set; }

        public byte? BuyStatus { get; set; }

        public int? VersionId { get; set; }

        /// <summary>
        /// 过期截止时间  <see cref="TenantExpiredDeadline"/>
        /// </summary>
        public byte? ExpiredDeadline { get; set; }

        public string Remark { get; set; }

        public int? AgentId { get; set; }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhereGet());
            if (!string.IsNullOrEmpty(Key))
            {
                condition.Append($" AND (AgentName LIKE '{Key}%' OR AgentPhone LIKE '{Key}%')");
            }
            if (!string.IsNullOrEmpty(TenantKey))
            {
                condition.Append($" AND (Name LIKE '{TenantKey}%' OR TenantCode LIKE '{TenantKey}%' OR Phone LIKE '{TenantKey}%')");
            }
            if (StartOt != null)
            {
                condition.Append($" AND Ot >= '{StartOt.Value.EtmsToDateString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND Ot < '{EndOt.Value.EtmsToDateString()}'");
            }
            if (Status != null)
            {
                switch (Status)
                {
                    case EmSysTenantStatus.Normal:
                        condition.Append($" AND [Status] = {Status.Value} AND ExDate >= '{DateTime.Now.Date}'");
                        break;
                    case EmSysTenantStatus.Expired:
                        condition.Append($" AND ExDate < '{DateTime.Now.Date}'");
                        break;
                    case EmSysTenantStatus.IsLock:
                    default:
                        condition.Append($" AND [Status] = {Status.Value}");
                        break;
                }
            }
            if (VersionId != null)
            {
                condition.Append($" AND VersionId = {VersionId.Value}");
            }
            if (ExpiredDeadline != null)
            {
                condition.Append($" AND ExDate >= '{DateTime.Now.Date}'");
                switch (ExpiredDeadline.Value)
                {
                    case TenantExpiredDeadline.MonthsThree:
                        condition.Append($" AND ExDate <= '{DateTime.Now.AddMonths(3).Date}'");
                        break;
                    case TenantExpiredDeadline.MonthsTwo:
                        condition.Append($" AND ExDate <= '{DateTime.Now.AddMonths(2).Date}'");
                        break;
                    case TenantExpiredDeadline.MonthsOne:
                        condition.Append($" AND ExDate <= '{DateTime.Now.AddMonths(1).Date}'");
                        break;
                    case TenantExpiredDeadline.Days15:
                        condition.Append($" AND ExDate <= '{DateTime.Now.AddDays(15).Date}'");
                        break;
                    case TenantExpiredDeadline.Day7:
                        condition.Append($" AND ExDate <= '{DateTime.Now.AddDays(7).Date}'");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(Remark))
            {
                condition.Append($" AND Remark LIKE '%{Remark}%'");
            }
            if (BuyStatus != null)
            {
                condition.Append($" AND BuyStatus = {BuyStatus.Value}");
            }
            if (AgentId != null)
            {
                condition.Append($" AND AgentId = {AgentId.Value}");
            }
            return condition.ToString();
        }
    }

    public struct TenantExpiredDeadline
    {
        /// <summary>
        /// 三个月
        /// </summary>
        public const byte MonthsThree = 0;

        /// <summary>
        /// 两个月
        /// </summary>
        public const byte MonthsTwo = 1;

        /// <summary>
        /// 一个月
        /// </summary>
        public const byte MonthsOne = 2;

        /// <summary>
        /// 十五天
        /// </summary>
        public const byte Days15 = 3;

        /// <summary>
        /// 7天
        /// </summary>
        public const byte Day7 = 4;
    }
}
