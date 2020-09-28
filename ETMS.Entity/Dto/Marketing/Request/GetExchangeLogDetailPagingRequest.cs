using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Marketing.Request
{
    public class GetExchangeLogDetailPagingRequest : RequestPagingBase
    {
        public string StudentKey { get; set; }

        public string GiftName { get; set; }

        public string No { get; set; }

        public byte? Status { get; set; }

        public long? StudentId { get; set; }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var condition = new StringBuilder(DataFilterWhere);
            if (!string.IsNullOrEmpty(StudentKey))
            {
                condition.Append($" AND (StudentName LIKE '{StudentKey}%' OR StudentPhone LIKE '{StudentKey}%')");
            }
            if (!string.IsNullOrEmpty(No))
            {
                condition.Append($" AND No = '{No}'");
            }
            if (!string.IsNullOrEmpty(GiftName))
            {
                condition.Append($" AND  GiftName LIKE '{GiftName}%'");
            }
            if (Status != null)
            {
                condition.Append($" AND [Status] = '{Status}'");
            }
            if (StudentId != null)
            {
                condition.Append($" AND StudentId = {StudentId.Value}");
            }
            return condition.ToString();
        }
    }
}
