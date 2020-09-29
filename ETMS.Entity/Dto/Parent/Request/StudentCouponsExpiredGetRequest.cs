using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class StudentCouponsExpiredGetRequest : ParentRequestPagingBase
    {
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            condition.Append($" AND [Status] = {EmCouponsStudentStatus.Unused} AND (ExpiredTime IS NOT NULL AND ExpiredTime < '{DateTime.Now.Date}')");
            return condition.ToString();
        }
    }
}
