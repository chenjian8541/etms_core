using ETMS.Entity.Dto.HisData.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Educational3.Output
{
    public class AchievementStudentIncreaseGetOutput
    {
        public EchartsBar<decimal> ScoreMyIncrease { get; set; }

        public EchartsBar<int> RankMyIncrease { get; set; }
    }
}
