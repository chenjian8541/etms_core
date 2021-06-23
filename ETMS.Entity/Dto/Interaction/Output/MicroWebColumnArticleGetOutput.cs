using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class MicroWebColumnArticleGetOutput
    {
        public long Id { get; set; }

        public long ColumnId { get; set; }

        public string ArTitile { get; set; }

        public string ArCoverImg { get; set; }

        public string ArCoverImgUrl { get; set; }

        public string ArSummary { get; set; }

        public string ArContent { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMicroWebStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 样式
        ///  <see cref="ETMS.Entity.Enum.EmMicroWebStyle"/>
        /// </summary>
        public int ShowStyle { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsShowYuYue { get; set; }

        public string ColumnName { get; set; }
    }
}
