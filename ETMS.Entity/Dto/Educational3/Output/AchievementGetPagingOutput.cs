using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational3.Output
{
    public class AchievementGetPagingOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }

        public long SubjectId { get; set; }

        public string SubjectName { get; set; }

        public decimal ScoreTotal { get; set; }

        public decimal ScoreMax { get; set; }

        public decimal ScoreMin { get; set; }

        public decimal ScoreAverage { get; set; }

        public int StudentCount { get; set; }

        public int StudentInCount { get; set; }

        public int StudentMissCount { get; set; }

        public int StudenReadCount { get; set; }

        public string ExamOt { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmAchievementSourceType"/>
        /// </summary>
        public int SourceType { get; set; }

        public string SourceTypeDesc { get; set; }

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

        public string StatusDesc { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsCalculate { get; set; }
    }
}
