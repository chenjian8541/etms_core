using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class MicroWebColumnArticleGetPagingOutput
    {
        public long Id { get; set; }

        public long ColumnId { get; set; }

        public string ArTitile { get; set; }

        public string ArCoverImg { get; set; }

        public string ArCoverImgUrl { get; set; }

        public string ArSummary { get; set; }

        public int ReadCount { get; set; }

        public int OrderIndex { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMicroWebStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
