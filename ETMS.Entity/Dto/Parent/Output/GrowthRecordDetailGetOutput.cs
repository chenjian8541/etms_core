using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class GrowthRecordDetailGetOutput
    {
        public int TenantId { get; set; }

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
        /// 创建时间
        /// </summary>
        public string OtDesc { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public bool IsThisYear { get; set; }

        public List<ParentCommentOutput> CommentOutputs { get; set; }
    }
}
