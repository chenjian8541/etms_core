using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveGrowthRecordGetOutput
    {
        public long GrowthRecordId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long CreateUserId { get; set; }

        public string CreateUserName { get; set; }

        /// <summary>
        /// 类型  <see cref="ETMS.Entity.Enum.EmActiveGrowthRecordType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        /// <summary>
        /// 关联ID
        /// ",1,2,"
        /// </summary>
        public string RelatedIds { get; set; }

        /// <summary>
        /// 关联信息
        /// </summary>
        public string RelatedDesc { get; set; }

        /// <summary>
        /// 成长档案类型
        /// </summary>
        public long GrowingTag { get; set; }

        public string GrowingTagDesc { get; set; }

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
        public List<string> GrowthMediasUrl { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime Ot { get; set; }

        /// <summary>
        /// 评论信息
        /// </summary>
        public List<CommentOutput> CommentOutputs { get; set; }
    }
}
