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

        public long? UserId { get; set; }

        public int? LastOpLimitMonth { get; set; }

        /// <summary>
        /// 过期年月
        /// </summary>
        public string ExpiredYearMonth { get; set; }


        /// <summary>
        /// 查询时间
        /// </summary>
        public List<string> OtLastRenewalTime { get; set; }

        private DateTime? _startLastRenewalTime;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartLastRenewalTime
        {
            get
            {
                if (_startLastRenewalTime != null)
                {
                    return _startLastRenewalTime;
                }
                if (OtLastRenewalTime == null || OtLastRenewalTime.Count == 0)
                {
                    return null;
                }
                _startLastRenewalTime = Convert.ToDateTime(OtLastRenewalTime[0]);
                return _startLastRenewalTime;
            }
        }

        private DateTime? _endLastRenewalTime;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndLastRenewalTime
        {
            get
            {
                if (_endLastRenewalTime != null)
                {
                    return _endLastRenewalTime;
                }
                if (OtLastRenewalTime == null || OtLastRenewalTime.Count < 2)
                {
                    return null;
                }
                _endLastRenewalTime = Convert.ToDateTime(OtLastRenewalTime[1]).AddDays(1); ;
                return _endLastRenewalTime;
            }
        }

        public int? AgtPayType { get; set; }

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
            if (UserId != null)
            {
                condition.Append($" AND UserId = {UserId.Value}");
            }
            if (StartLastRenewalTime != null)
            {
                condition.Append($" AND LastRenewalTime >= '{StartLastRenewalTime.Value.EtmsToDateString()}'");
            }
            if (EndLastRenewalTime != null)
            {
                condition.Append($" AND LastRenewalTime < '{EndLastRenewalTime.Value.EtmsToDateString()}'");
            }
            if (AgtPayType != null)
            {
                condition.Append($" AND AgtPayType = {AgtPayType.Value}");
            }
            if (LastOpLimitMonth != null)
            {
                var time = DateTime.Now.AddMonths(-LastOpLimitMonth.Value);
                var timeDesc = time.EtmsToString();
                condition.Append($" AND (LastOpTime <= '{timeDesc}' OR LastOpTime IS NULL) AND Ot <= '{timeDesc}'");
            }
            if (!string.IsNullOrEmpty(ExpiredYearMonth))
            {
                var time = Convert.ToDateTime(ExpiredYearMonth);
                var minDate = new DateTime(time.Year, time.Month, 1);
                var maxDate = minDate.AddMonths(1);
                condition.Append($" AND ExDate >= '{minDate.EtmsToDateString()}' AND ExDate < '{maxDate.EtmsToDateString()}'");
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
