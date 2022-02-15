using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class StudentClassTimetableRequest : ParentRequestBase
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

        /// <summary>
        /// 是否为请假
        /// </summary>
        public bool IsReqLeave { get; set; }

        protected override string DataFilterWhere(string studentFieldName = "StudentId")
        {
            if (ParentStudentIds.Count == 0)
            {
                return "1=2";
            }
            if (ParentStudentIds.Count == 1)
            {
                return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal} AND (StudentIdsTemp LIKE '%,{ParentStudentIds[0]},%' OR StudentIdsReservation LIKE '%,{ParentStudentIds[0]},%' OR StudentIdsClass LIKE '%,{ParentStudentIds[0]},%')";
            }
            var studentLimit = new StringBuilder($"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal} AND (");
            for (var i = 0; i < ParentStudentIds.Count; i++)
            {
                if (i == ParentStudentIds.Count - 1)
                {
                    studentLimit.Append($"StudentIdsTemp LIKE '%,{ParentStudentIds[i]},%' OR StudentIdsReservation LIKE '%,{ParentStudentIds[i]},%' OR StudentIdsClass LIKE '%,{ParentStudentIds[i]},%' )");
                }
                else
                {
                    studentLimit.Append($"StudentIdsTemp LIKE '%,{ParentStudentIds[i]},%' OR StudentIdsReservation LIKE '%,{ParentStudentIds[i]},%' OR StudentIdsClass LIKE '%,{ParentStudentIds[i]},%' OR ");
                }
            }
            return studentLimit.ToString();
        }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            if (StartOt != null)
            {
                condition.Append($" AND ClassOt >= '{StartOt.Value.EtmsToDateString()}'");
            }
            if (EndOt != null)
            {
                condition.Append($" AND ClassOt < '{EndOt.Value.EtmsToDateString()}'");
            }
            if (IsReqLeave)
            {
                condition.Append($" AND ClassOt >= '{DateTime.Now.EtmsToDateString()}' AND [Status] = {EmClassTimesStatus.UnRollcall}");
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
