using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveGrowthRecordStudentStatusGetOutput
    {
        public List<ActiveGrowthRecordStudentStatusItem> ReadList { get; set; }

        public List<ActiveGrowthRecordStudentStatusItem> UnReadList { get; set; }
    }

    public class ActiveGrowthRecordStudentStatusItem
    {
        public long Id { get; set; }

        public string StudentName { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReadStatus { get; set; }

        /// <summary>
        /// 收藏状态   <see cref="ETMS.Entity.Enum.EmActiveGrowthRecordDetailFavoriteStatus"/>
        /// </summary>
        public byte FavoriteStatus { get; set; }
    }
}
