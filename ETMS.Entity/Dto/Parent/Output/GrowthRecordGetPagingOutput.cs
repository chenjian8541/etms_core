using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class GrowthRecordGetPagingOutput
    {
        public int TenantId { get; set; }

        public long GrowthRecordId { get; set; }

        public long GrowthRecordDetailId { get; set; }

        /// <summary>
        /// 学员ID
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

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

        public int Month { get; set; }

        public int Day { get; set; }

        public bool IsThisYear { get; set; }

        /// <summary>
        /// 场景类型
        /// <see cref="ETMS.Entity.Enum.EmActiveGrowthRecordDetailSceneType"/>
        /// </summary>
        public int SceneType { get; set; }

        /// <summary>
        /// 非成长档案的情况下，所关联的ID
        /// </summary>
        public long? RelatedId { get; set; }
    }
}
