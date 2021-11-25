using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ActiveHomeworkAdd2Request : RequestBase
    {
        /// <summary>
        /// 作业标题
        /// </summary>
        public string Title { get; set; }

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

        /// <summary>
        /// 查询时间
        /// </summary>
        public List<string> Ot { get; set; }

        public DateTime LxStartDate { get; set; }

        public DateTime LxEndDate { get; set; }

        public int? LxExTime { get; set; }

        public double LxTotalCount { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(Title))
            {
                return "请输入作业标题";
            }
            if (ClassInfos == null || ClassInfos.Count == 0)
            {
                return "请选择发送班级";
            }
            if (Ot == null || Ot.Count < 2)
            {
                return "请选择起止日期";
            }
            LxStartDate = Convert.ToDateTime(Ot[0]);
            LxEndDate = Convert.ToDateTime(Ot[1]);
            if (LxStartDate < DateTime.Now.Date)
            {
                return "发布日期必须大于等于当前日期";
            }
            if (LxStartDate > LxEndDate)
            {
                return "截止日期必须大于发布日期";
            }
            LxTotalCount = (LxEndDate - LxStartDate).TotalDays + 1;
            if (LxTotalCount > 100)
            {
                return "最多布置100天的连续作业";
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
