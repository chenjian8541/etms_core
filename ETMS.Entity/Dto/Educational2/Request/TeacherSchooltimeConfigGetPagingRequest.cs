using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational2.Request
{
    public class TeacherSchooltimeConfigGetPagingRequest : RequestPagingBase, IDataLimit
    {
        /// <summary>
        /// 姓名/手机号码
        /// </summary>
        public string Key { get; set; }

        public string GetDataLimitFilterWhere()
        {
            return $" AND Id = {LoginUserId}";
        }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND IsTeacher = {EmBool.True}");
            if (!string.IsNullOrEmpty(Key))
            {
                condition.Append($" AND (Name LIKE '%{Key}%' OR Phone LIKE '{Key}%')");
            }
            if (IsDataLimit)
            {
                condition.Append(GetDataLimitFilterWhere());
            }
            return condition.ToString();
        }
    }
}
