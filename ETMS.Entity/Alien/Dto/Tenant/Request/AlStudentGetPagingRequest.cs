using ETMS.Entity.Alien.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.Tenant.Request
{
    public class AlStudentGetPagingRequest : AlienTenantRequestPagingBase
    {
        public string StudentKey { get; set; }

        public byte? StudentType { get; set; }

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
                _endOt = Convert.ToDateTime(Ot[1]).AddDays(1);
                return _endOt;
            }
        }

        public string HomeAddress { get; set; }

        public int? BirthdayMonth { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(TenantDataFilterWhere());
            if (StudentType != null)
            {
                condition.Append($" AND StudentType = {StudentType.Value}");
            }
            if (StartOt != null)
            {
                condition.Append($" AND Ot >= '{StartOt.Value.EtmsToString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND Ot < '{EndOt.Value.EtmsToString()}'");
            }
            if (!string.IsNullOrEmpty(StudentKey))
            {
                condition.Append($" AND (Name LIKE '%{StudentKey}%' OR Phone LIKE '{StudentKey}%' OR PhoneBak LIKE '{StudentKey}%' OR CardNo = '{StudentKey}' OR NamePinyin = '{StudentKey.ToLower()}')");
            }
            if (BirthdayMonth != null)
            {
                condition.Append($" AND BirthdayMonth = {BirthdayMonth}");
            }
            if (!string.IsNullOrEmpty(HomeAddress))
            {
                condition.Append($" AND  HomeAddress LIKE '%{HomeAddress}%'");
            }
            return condition.ToString();
        }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            if (StartOt != null && EndOt != null && StartOt >= EndOt)
            {
                return "开始时间不能大于结束时间";
            }
            return base.Validate();
        }
    }
}
