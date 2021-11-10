using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtActiveGrowthRecord")]
    public class EtActiveGrowthRecord : Entity<long>
    {
        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmActiveGrowthRecordType"/>
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 关联ID
        /// ",1,2,"
        /// </summary>
        public string RelatedIds { get; set; }

        /// <summary>
        /// 成长档案类型
        /// </summary>
        public long GrowingTag { get; set; }

        /// <summary>
        /// 推送类型   <see cref="ETMS.Entity.Enum.EmActiveGrowthRecordSendType"/>
        /// </summary>
        public byte SendType { get; set; }

        /// <summary>
        /// 成长档案内容
        /// </summary>
        public string GrowthContent { get; set; }

        /// <summary>
        /// 成长档案媒体文件
        /// </summary>
        public string GrowthMedias { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte ReadStatus { get; set; }

        /// <summary>
        /// 收藏状态   <see cref="ETMS.Entity.Enum.EmActiveGrowthRecordDetailFavoriteStatus"/>
        /// </summary>
        public byte FavoriteStatus { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 已读数量
        /// </summary>
        public int ReadCount { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime Ot { get; set; }
    }
}
