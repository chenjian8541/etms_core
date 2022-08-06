using ETMS.Entity.Dto.Common.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Open3.Output
{
    public class AchievementDetailGetOutput
    {
        public string StudentName { get; set; }

        public string StudentAvatar { get; set; }

        public string Name { get; set; }

        public string ExamOt { get; set; }

        public string ScoreMy { get; set; }

        public int RankMy { get; set; }

        public string ScoreMax { get; set; }

        public string ScoreAverage { get; set; }

        public string Comment { get; set; }

        public byte CheckStatus { get; set; }

        public byte ShowRankParent { get; set; }

        public string SubjectName { get; set; }

        public string ScoreTotal { get; set; }

        /// <summary>
        /// 性别  <see cref="ETMS.Entity.Enum.EmGender"/>
        /// </summary>
        public byte? StudentGender { get; set; }

        public ShareContent ShareContent { get; set; }
    }
}
