using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveHomeworkAddRequest : RequestBase
    {
        /// <summary>
        /// 作业标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 截至时间
        /// </summary>
        public DateTime? ExDate { get; set; }

        /// <summary>
        /// 发送班级
        /// </summary>
        public List<ActiveHomeworkAddClassInfo> ClassInfos { get; set; }

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
            if (ExDate != null && ExDate.Value <= DateTime.Now)
            {
                return "截止时间必须大于当前时间";
            }
            if (ClassInfos == null || ClassInfos.Count == 0)
            {
                return "请选择发送班级";
            }
            foreach (var p in ClassInfos)
            {
                var msg = p.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    return msg;
                }
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
