using ETMS.Entity.Alien.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Alien.Dto.TenantStatistics.Request
{
    public class AlienTenantClassRecordGetPagingRequest : AlienTenantRequestPagingBase
    {
        public long? ClassCategoryId { get; set; }

        public long? ClassId { get; set; }

        public long? StudentId { get; set; }

        public long? TeacherId { get; set; }

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

        public long? ClassRoomId { get; set; }

        public long? CourseId { get; set; }

        public byte? Status { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(TenantDataFilterWhere());
            if (ClassId != null)
            {
                condition.Append($" AND ClassId = {ClassId.Value}");
            }
            if (StudentId != null)
            {
                condition.Append($" AND StudentIds LIKE '%,{StudentId.Value},%'");
            }
            if (TeacherId != null)
            {
                condition.Append($" AND Teachers LIKE '%,{TeacherId.Value},%'");
            }
            if (ClassRoomId != null)
            {
                condition.Append($" AND ClassRoomIds LIKE '%,{ClassRoomId.Value},%'");
            }
            if (StartOt != null)
            {
                condition.Append($" AND ClassOt >= '{StartOt.Value.EtmsToDateString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND ClassOt < '{EndOt.Value.EtmsToDateString()}'");
            }
            if (CourseId != null)
            {
                condition.Append($" AND CourseList LIKE '%,{CourseId.Value},%'");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value}");
            }
            if (ClassCategoryId != null)
            {
                condition.Append($" AND [ClassCategoryId] = {ClassCategoryId.Value}");
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
