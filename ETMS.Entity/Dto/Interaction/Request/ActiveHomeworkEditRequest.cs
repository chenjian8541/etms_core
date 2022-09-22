using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveHomeworkEditRequest : RequestBase
    {
        public long CId { get; set; }

        /// <summary>
        /// 作业标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 作业要求
        /// </summary>
        public string WorkContent { get; set; }

        /// <summary>
        /// 媒体文件
        /// </summary>
        public List<string> WorkMediasKeys { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Title))
            {
                return "请输入作业标题";
            }
            if (string.IsNullOrEmpty(WorkContent) && (WorkMediasKeys == null || WorkMediasKeys.Count == 0))
            {
                return "请填写作业要求";
            }
            if (WorkMediasKeys != null && WorkMediasKeys.Count > 30)
            {
                return "最多保存30个媒体文件";
            }
            return base.Validate();
        }
    }
}
