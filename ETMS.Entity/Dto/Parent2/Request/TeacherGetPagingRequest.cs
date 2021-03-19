using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent2.Request
{
    public class TeacherGetPagingRequest : ParentRequestPagingBase
    {
        public string Key { get; set; }

        protected override string DataFilterWhere(string studentFieldName = "StudentId")
        {
            return $"TenantId = {LoginTenantId} AND IsDeleted = {EmIsDeleted.Normal}";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            condition.Append($" AND IsTeacher = {EmBool.True}");
            if (!string.IsNullOrEmpty(Key))
            {
                condition.Append($" AND (Name LIKE '%{Key}%' OR NickName LIKE '%{Key}%')");
            }
            return condition.ToString();
        }
    }
}
