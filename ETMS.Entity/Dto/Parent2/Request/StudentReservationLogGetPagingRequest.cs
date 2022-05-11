using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent2.Request
{
    public class StudentReservationLogGetPagingRequest : ParentRequestPagingBase
    {
        public long StudentId { get; set; }

        /// <summary>
        /// 状态 <see cref="ETMS.Entity.Enum.EmStudentReservationLogOutputStatus"/>
        /// </summary>
        public byte? LogStatus { get; set; }

        protected override string DataFilterWhere(string studentFieldName = "StudentId")
        {
            return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal} ";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            var now = DateTime.Now.EtmsToDateString();
            if (LogStatus == EmStudentReservationLogOutputStatus.Normal)
            {
                condition.Append($" AND [Status] = {EmClassTimesStatus.UnRollcall} AND ClassOt >= '{now}'");
            }
            else
            {
                condition.Append($" AND ([Status] = {EmClassTimesStatus.BeRollcall} OR ClassOt < '{now}')");
            }
            condition.Append($" AND StudentIdsReservation LIKE '%,{StudentId},%'");
            return condition.ToString();
        }

        public override string Validate()
        {
            if (StudentId <= 0 || LogStatus == null || LogStatus < 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
