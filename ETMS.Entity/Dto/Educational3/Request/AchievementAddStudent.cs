using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational3.Request
{
    public class AchievementAddStudent
    {
        public long StudentId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmAchievementDetailCheckStatus"/>
        /// </summary>
        public byte CheckStatus { get; set; }

        public decimal ScoreMy { get; set; }

        public string Comment { get; set; }
    }
}
