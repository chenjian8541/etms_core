using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent5.Request
{
    public class StudentTryClassGetPagingRequest: ParentRequestPagingBase
    {
        public long? StudentId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            if (StudentId != null)
            {
                condition.Append($" AND StudentId = {StudentId}");
            }
            return condition.ToString();
        }

    }
}
