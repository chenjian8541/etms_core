using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesGetMyOtRequest : RequestBase
    {
        public long TeacherId { get; set; }

        public bool IsOnlyReservation { get; set; }

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
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND Teachers LIKE '%,{TeacherId},%'");
            if (StartOt != null)
            {
                condition.Append($" AND ClassOt >= '{StartOt.Value.EtmsToDateString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND ClassOt < '{EndOt.Value.EtmsToDateString()}'");
            }
            if (IsOnlyReservation)
            {
                condition.Append($" AND ReservationType = {EmBool.True} ");
            }
            return condition.ToString();
        }

        public override string Validate()
        {
            if (Ot == null || Ot.Count != 2)
            {
                return "时间格式不正确";
            }
            return string.Empty;
        }
    }
}

