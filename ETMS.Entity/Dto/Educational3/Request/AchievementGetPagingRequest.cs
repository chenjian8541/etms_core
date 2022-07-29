using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational3.Request
{
    public class AchievementGetPagingRequest : RequestPagingBase, IDataLimit
    {
        public string Name { get; set; }

        public long? SubjectId { get; set; }

        public byte? Status { get; set; }

        public string GetDataLimitFilterWhere()
        {
            return $" AND UserId = {LoginUserId}";
        }

        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name LIKE '%{Name}%'");
            }
            if (SubjectId != null)
            {
                condition.Append($" AND SubjectId = {SubjectId}");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = {Status}");
            }
            if (IsDataLimit)
            {
                condition.Append(GetDataLimitFilterWhere());
            }
            return condition.ToString();
        }
    }
}
