using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational3.Request
{
    public class AchievementEditRequest : RequestBase
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public byte ShowRankParent { get; set; }

        public byte ShowParent { get; set; }

        public AchievementStudentChange StudentChange { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入考试名称";
            }
            return string.Empty;
        }
    }

    public class AchievementStudentChange {

        public List<long> DelDetailIds { get; set; }

        public List<AchievementStudentEdit> Edits { get; set; }
    }

    public class AchievementStudentEdit{
        public long DetailId { get; set; }

        public long StudentId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmAchievementDetailCheckStatus"/>
        /// </summary>
        public byte CheckStatus { get; set; }

        public decimal ScoreMy { get; set; }

        public string Comment { get; set; }
    }
}
