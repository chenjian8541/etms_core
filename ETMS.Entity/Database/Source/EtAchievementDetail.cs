using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtAchievementDetail")]
    public class EtAchievementDetail : Entity<long>
    {
        public long AchievementId { get; set; }

        public long StudentId { get; set; }

        public string Name { get; set; }

        public long SubjectId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmAchievementDetailCheckStatus"/>
        /// </summary>
        public byte CheckStatus { get; set; }

        public decimal ScoreTotal { get; set; }

        public decimal ScoreMy { get; set; }

        public int RankMy { get; set; }

        public string Comment { get; set; }

        public DateTime ExamOt { get; set; }

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

        public DateTime CreateTime { get; set; }

        public long UserId { get; set; }
    }
}
