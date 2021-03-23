using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveGrowthStudentGetPagingRequest : RequestPagingBase
    {
        public long StudentId { get; set; }

        public long? GrowingTagId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND StudentId = {StudentId}");
            if (GrowingTagId != null)
            {
                condition.Append($" AND GrowingTag = {GrowingTagId.Value}");
            }

            return condition.ToString();
        }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
