using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.User.Request
{
    public class UserGetPagingRequest : RequestPagingBase
    {
        /// <summary>
        /// 姓名/手机号码
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int? RoleId { get; set; }

        public int? JobType { get; set; }

        /// <summary>
        /// 只看老师
        /// </summary>
        public bool OnlyShowTeacher { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Key))
            {
                condition.Append($" AND (Name LIKE '%{Key}%' OR Phone LIKE '{Key}%')");
            }
            if (OnlyShowTeacher)
            {
                condition.Append(" AND IsTeacher = 1");
            }
            if (RoleId != null)
            {
                condition.Append($" AND RoleId = {RoleId}");
            }
            if (JobType != null)
            {
                condition.Append($" AND JobType = {JobType}");
            }
            return condition.ToString();
        }

        /// <summary>
        /// 数据校验
        /// </summary>
        /// <returns></returns>
        public override string Validate()
        {
            return base.Validate();
        }
    }
}
