using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational3.Request
{
    public class AchievementDetailEditRequest : RequestBase
    {
        public long DetailId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmAchievementDetailCheckStatus"/>
        /// </summary>
        public byte CheckStatus { get; set; }

        public decimal ScoreMy { get; set; }

        public string Comment { get; set; }

        public override string Validate()
        {
            if (DetailId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
