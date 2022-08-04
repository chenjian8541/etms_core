using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent5.Output
{
    public class AchievementDetailGetPagingOutput
    {
        public long CId { get; set; }

        public string StudentName { get; set; }

        public string Name { get; set; }

        public string SubjectName { get; set; }

        public string ExamOt { get; set; }

        public byte CheckStatus { get; set; }

        public string ScoreTotal { get; set; }

        public string ScoreMy { get; set; }

        public string Comment { get; set; }
    }
}
