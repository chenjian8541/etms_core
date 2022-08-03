using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational3.Output
{
    public class AchievementDetailGetPagingOutput
    {
        public long CId { get; set; }

        public long AchievementId { get; set; }

        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string Name { get; set; }

        public long SubjectId { get; set; }

        public string SubjectName { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmAchievementDetailCheckStatus"/>
        /// </summary>
        public byte CheckStatus { get; set; }

        public string CheckStatusDesc { get; set; }
        public decimal ScoreTotal { get; set; }

        public decimal ScoreMy { get; set; }

        public int RankMy { get; set; }

        public string Comment { get; set; }

        public string ExamOt { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmAchievementSourceType"/>
        /// </summary>
        public int SourceType { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ShowParent { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ShowRankParent { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmAchievementStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReadStatus { get; set; }

    }
}
