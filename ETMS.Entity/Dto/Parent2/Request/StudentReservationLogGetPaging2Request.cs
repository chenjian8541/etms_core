using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent2.Request
{
    public class StudentReservationLogGetPaging2Request : ParentRequestPagingBase
    {
        public long StudentId { get; set; }

        protected override string DataFilterWhere(string studentFieldName = "StudentId")
        {
            return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal} AND StudentId = {StudentId}";
        }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            return condition.ToString();
        }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
