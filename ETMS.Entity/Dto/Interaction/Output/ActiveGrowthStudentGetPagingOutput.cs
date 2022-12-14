using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveGrowthStudentGetPagingOutput
    {
        public long GrowthRecordId { get; set; }

        public long GrowthRecordDetailId { get; set; }

        /// <summary>
        /// 成长档案类型
        /// </summary>
        public long GrowingTag { get; set; }

        public string GrowingTagDesc { get; set; }

        /// <summary>
        /// 成长档案内容
        /// </summary>
        public string GrowthContent { get; set; }

        public List<string> GrowthMediasUrl { get; set; }

        /// <summary>
        /// 收藏状态   <see cref="ETMS.Entity.Enum.EmActiveGrowthRecordDetailFavoriteStatus"/>
        /// </summary>
        public byte FavoriteStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string OtDesc { get; set; }

        public string UserName { get; set; }
    }
}
