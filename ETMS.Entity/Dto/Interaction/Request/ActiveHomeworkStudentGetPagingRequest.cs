using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveHomeworkStudentGetPagingRequest : RequestPagingBase
    {
        public long StudentId { get; set; }

        public byte? AnswerStatus { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            condition.Append($" AND StudentId = {StudentId}");
            if (AnswerStatus != null)
            {
                condition.Append($" AND [AnswerStatus] = {AnswerStatus.Value}");
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
