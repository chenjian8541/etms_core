using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class TeacherClassTimesGetPagingRequest : RequestPagingBase
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public string UserKey { get; set; }

        public string DateDesc { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(UserKey))
            {
                condition.Append($" AND (UserName LIKE '{UserKey}%' OR UserPhone LIKE '{UserKey}%')");
            }
            if (!string.IsNullOrEmpty(DateDesc))
            {
                condition.Append($" AND FirstTime = '{DateDesc}-01'");
            }
            return condition.ToString();
        }
    }
}
