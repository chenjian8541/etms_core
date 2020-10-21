using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class HomeworkUnansweredGetPagingRequest : ParentRequestPagingBase
    {
        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere());
            condition.Append($" AND AnswerStatus = {EmActiveHomeworkDetailAnswerStatus.Unanswered}");
            return condition.ToString();
        }
    }
}
