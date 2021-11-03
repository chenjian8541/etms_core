using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentTrackLogGetPagingRequest : RequestPagingBase
    {
        public string StudentKey { get; set; }

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

        public byte? ContentType { get; set; }

        public bool IsTodayMustTrack { get; set; }

        public string TrackContent { get; set; }

        public long? TrackUserId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (StartOt != null)
            {
                condition.Append($" AND TrackTime >= '{StartOt.Value.EtmsToString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND TrackTime < '{EndOt.Value.EtmsToString()}'");
            }
            if (!string.IsNullOrEmpty(StudentKey))
            {
                condition.Append($" AND (StudentName LIKE '{StudentKey}%' OR StudentPhone LIKE '{StudentKey}%' OR StudentPhoneBak LIKE '{StudentKey}%')");
            }
            if (ContentType != null)
            {
                condition.Append($" AND ContentType = {ContentType.Value}");
            }
            if (IsTodayMustTrack)
            {
                condition.Append($" AND NextTrackTime = '{DateTime.Now.EtmsToDateString()}'");
            }
            if (!string.IsNullOrEmpty(TrackContent))
            {
                condition.Append($" AND TrackContent LIKE '%{TrackContent}%'");
            }
            if (TrackUserId != null)
            {
                condition.Append($" AND TrackUserId = {TrackUserId.Value}");
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
