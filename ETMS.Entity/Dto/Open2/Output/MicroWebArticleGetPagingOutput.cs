using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open2.Output
{
    public class MicroWebArticleGetPagingOutput
    {
        public long Id { get; set; }

        public long ColumnId { get; set; }

        public string ArTitile { get; set; }

        public string ArCoverImgUrl { get; set; }

        public string ArSummary { get; set; }

        /// <summary>
        /// 样式
        ///  <see cref="ETMS.Entity.Enum.EmMicroWebStyle"/>
        /// </summary>
        public int ShowStyle { get; set; }
    }
}
