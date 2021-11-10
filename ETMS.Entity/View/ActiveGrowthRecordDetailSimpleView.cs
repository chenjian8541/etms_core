using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.View
{
    public class ActiveGrowthRecordDetailSimpleView
    {
        public long Id { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

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
