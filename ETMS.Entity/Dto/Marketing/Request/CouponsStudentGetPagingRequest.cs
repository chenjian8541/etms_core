using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class CouponsStudentGetPagingRequest : RequestPagingBase
    {
        public string StudentKey { get; set; }

        public string CouponsTitle { get; set; }

        public int? Status { get; set; }

        public long? StudentId { set; get; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(CouponsTitle))
            {
                condition.Append($" AND CouponsTitle like '{CouponsTitle}%'");
            }
            if (!string.IsNullOrEmpty(StudentKey))
            {
                condition.Append($" AND (StudentName like '{StudentKey}%' or StudentPhone like '{StudentKey}%')");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status}");
            }
            if (StudentId != null)
            {
                condition.Append($" AND StudentId = {StudentId}");
            }
            return condition.ToString();
        }
    }
}
