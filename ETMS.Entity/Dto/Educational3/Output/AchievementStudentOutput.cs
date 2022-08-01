using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational3.Output
{
    public class AchievementStudentOutput
    {
        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        public long StudentId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmAchievementDetailCheckStatus"/>
        /// </summary>
        public byte CheckStatus { get; set; }

        public decimal ScoreMy { get; set; }

        public string Comment { get; set; }

        public bool IsChanged { get; set; }

        /// <summary>
        ///  <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReadStatus { get; set; }
    }
}
