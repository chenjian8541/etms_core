using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;
using ETMS.Entity.Enum;

namespace ETMS.Entity.Database.Source
{
    [Table("EtAchievement")]
    public class EtAchievement : Entity<long>
    {
        public string Name { get; set; }

        public long SubjectId { get; set; }

        public decimal ScoreTotal { get; set; }

        public decimal ScoreMax { get; set; }

        public decimal ScoreMin { get; set; }

        public decimal ScoreAverage { get; set; }

        public int StudentCount { get; set; }

        public int StudentInCount { get; set; }

        public int StudentMissCount { get; set; }

        public int StudenReadCount { get; set; }

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

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsCalculate { get; set; }
    }
}
