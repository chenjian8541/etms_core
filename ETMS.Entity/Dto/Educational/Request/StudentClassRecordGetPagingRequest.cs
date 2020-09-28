using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class StudentClassRecordGetPagingRequest : RequestPagingBase
    {
        public long StudentId { get; set; }

        public byte? Status { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND StudentId = {StudentId}");
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status.Value}");
            }
            return condition.ToString();
        }
    }
}
