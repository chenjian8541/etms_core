using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class TryCalssLogGetPagingRequest : RequestPagingBase, IDataLimit
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

        public long? StudentId { get; set; }

        public long? CourseId { get; set; }

        public int? Status { get; set; }

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
                condition.Append($" AND ClassOt >= '{StartOt.Value.EtmsToDateString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND ClassOt < '{EndOt.Value.EtmsToDateString()}'");
            }
            if (StudentId != null)
            {
                condition.Append($" AND StudentId = {StudentId.Value}");
            }
            if (CourseId != null)
            {
                condition.Append($" AND CourseId = {CourseId.Value}");
            }
            if (Status != null)
            {
                if (Status.Value == EmTryCalssLogStatus.IsExpired)
                {
                    condition.Append($" AND ([Status] = {Status.Value} OR ([Status] = {EmTryCalssLogStatus.IsBooked} AND (ClassOt IS NULL OR ClassOt < '{DateTime.Now.EtmsToDateString()}')))");
                }
                else if (Status.Value == EmTryCalssLogStatus.IsBooked)
                {
                    condition.Append($"  AND [Status] = {Status.Value} AND ClassOt IS NOT NULL AND ClassOt >= '{DateTime.Now.EtmsToDateString()}'");
                }
                else
                {
                    condition.Append($" AND [Status] = {Status.Value}");
                }
            }
            if (GetIsDataLimit(8))
            {
                condition.Append(GetDataLimitFilterWhere());
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
