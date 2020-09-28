using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassGetPagingRequest : RequestPagingBase, IDataLimit
    {
        public byte? Type { get; set; }

        public long? ClassCategoryId { get; set; }

        public long? CourseId { get; set; }

        public long? TeacherId { get; set; }

        public long? ClassRoomId { get; set; }

        public byte? ScheduleStatus { get; set; }

        public byte? CompleteStatus { get; set; }

        public string Name { get; set; }

        public bool IsGetOneToOneStudent { get; set; } = true;

        public string GetDataLimitFilterWhere()
        {
            return $" AND UserId = {LoginUserId}";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND DataType = {EmClassDataType.Normal}");
            if (Type != null)
            {
                condition.Append($" AND Type = {Type}");
            }
            if (!string.IsNullOrEmpty(Name))
            {
                condition.Append($" AND Name LIKE '%{Name}%'");
            }
            if (ClassCategoryId != null)
            {
                condition.Append($" AND ClassCategoryId = {ClassCategoryId.Value}");
            }
            if (CourseId != null)
            {
                condition.Append($" AND (CourseList = ',{CourseId.Value},' OR CourseList LIKE '%,{CourseId.Value},%')");
            }
            if (TeacherId != null)
            {
                condition.Append($" AND (Teachers = ',{TeacherId.Value},' OR Teachers LIKE '%,{TeacherId.Value},%')");
            }
            if (ClassRoomId != null)
            {
                condition.Append($" AND (ClassRoomIds = ',{ClassRoomId.Value},' OR  ClassRoomIds LIKE '%,{ClassRoomId.Value},%')");
            }
            if (ScheduleStatus != null)
            {
                condition.Append($" AND ScheduleStatus = {ScheduleStatus.Value}");
            }
            if (CompleteStatus != null)
            {
                condition.Append($" AND CompleteStatus = {CompleteStatus.Value}");
            }
            if (IsDataLimit)
            {
                condition.Append(GetDataLimitFilterWhere());
            }
            return condition.ToString();
        }
    }
}
