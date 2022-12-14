using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentCheckOnLogGetPagingRequest : RequestPagingBase
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
                _endOt = Convert.ToDateTime(Ot[1]);
                if (_endOt.Value.Hour == 0 && _endOt.Value.Minute == 0 && _endOt.Value.Second == 0)
                {
                    _endOt = _endOt.Value.AddDays(1);
                }
                return _endOt;
            }
        }

        public long? StudentId { get; set; }

        public byte? CheckType { get; set; }

        public long? ClassId { get; set; }

        public long? CourseId { get; set; }

        public byte? Status { get; set; }

        public long? TrackUser { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (StartOt != null)
            {
                condition.Append($" AND CheckOt >= '{StartOt.Value.EtmsToString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND CheckOt <= '{EndOt.Value.EtmsToString()}'");
            }
            if (StudentId != null)
            {
                condition.Append($" AND StudentId = {StudentId.Value}");
            }
            if (CheckType != null)
            {
                condition.Append($" AND CheckType = {CheckType.Value}");
            }
            if (ClassId != null)
            {
                condition.Append($" AND ClassId = {ClassId.Value}");
            }
            if (CourseId != null)
            {
                condition.Append($" AND CourseId = {CourseId.Value}");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value}");
            }
            if (TrackUser != null)
            {
                condition.Append($" AND TrackUser = {TrackUser.Value}");
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
