using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveHomeworkGetForEditOutput
    {
        public long HomeworkId { get; set; }

        /// <summary>
        /// 作业标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 作业要求
        /// </summary>
        public string WorkContent { get; set; }

        public List<Img> WorkMediasKeys { get; set; }
    }
}
