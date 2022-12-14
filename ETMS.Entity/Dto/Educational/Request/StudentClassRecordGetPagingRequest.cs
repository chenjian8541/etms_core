using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class StudentClassRecordGetPagingRequest : RequestPagingBase, IDataLimit
    {
        public long? StudentId { get; set; }

        public byte? Status { get; set; }

        public byte? StudentCheckStatus { get; set; }

        public long? CourseId { get; set; }

        public long? ClassId { get; set; }

        public long? TeacherId { get; set; }

        public string Remark { get; set; }

        public byte? ExceedClassTimesType { get; set; }

        public bool IsGetStudent { get; set; }

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
            return $" AND Teachers LIKE '%,{LoginUserId},%'";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (StudentId != null)
            {
                condition.Append($" AND StudentId = {StudentId}");
            }
            //if (Status != null)
            //{
            //    condition.Append($" AND [Status] = {Status.Value}");
            //}
            condition.Append($" AND [Status] = {EmClassRecordStatus.Normal}");
            if (StudentCheckStatus != null)
            {
                condition.Append($" AND StudentCheckStatus = {StudentCheckStatus.Value}");
            }
            if (CourseId != null)
            {
                condition.Append($" AND [CourseId] = {CourseId.Value}");
            }
            if (ClassId != null)
            {
                condition.Append($" AND ClassId = {ClassId.Value}");
            }
            if (TeacherId != null)
            {
                condition.Append($" AND Teachers LIKE '%,{TeacherId.Value},%'");
            }
            if (ExceedClassTimesType != null)
            {
                if (ExceedClassTimesType == 0)
                {
                    condition.Append(" AND ExceedClassTimes = 0");
                }
                else
                {
                    condition.Append(" AND ExceedClassTimes > 0");
                }
            }
            if (StartOt != null)
            {
                condition.Append($" AND ClassOt >= '{StartOt.Value.EtmsToDateString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND ClassOt < '{EndOt.Value.EtmsToDateString()}'");
            }
            if (!string.IsNullOrEmpty(Remark))
            {
                condition.Append($" AND Remark LIKE '%{Remark}%'");
            }
            if (GetIsDataLimit(3))
            {
                condition.Append(GetDataLimitFilterWhere());
            }
            return condition.ToString();
        }
    }
}
