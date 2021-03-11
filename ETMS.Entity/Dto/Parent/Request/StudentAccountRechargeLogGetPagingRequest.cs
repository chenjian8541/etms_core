using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class StudentAccountRechargeLogGetPagingRequest : ParentRequestPagingBase
    {
        public long StudentAccountRechargeId { get; set; }

        protected override string DataFilterWhere(string studentFieldName = "StudentId")
        {
            return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal} AND [Status] = {EmStudentAccountRechargeLogStatus.Normal} ";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            condition.Append($" AND StudentAccountRechargeId = {StudentAccountRechargeId}");
            return condition.ToString();
        }

        public override string Validate()
        {
            if (StudentAccountRechargeId <= 0)
            {
                return "请求数据不合法";
            }
            return base.Validate();
        }
    }
}
