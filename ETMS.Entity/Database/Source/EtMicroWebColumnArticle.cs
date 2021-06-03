using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Source
{
    [Table("EtMicroWebColumnArticle")]
    public class EtMicroWebColumnArticle : Entity<long>
    {
        public long ColumnId { get; set; }

        public string ArTitile { get; set; }

        public string ArCoverImg { get; set; }

        public string ArSummary { get; set; }

        public string ArContent { get; set; }

        public int ReadCount { get; set; }

        public int OrderIndex { get; set; }

        public DateTime CreateTime { get; set; }

        public long CreateUserId { get; set; }

        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmMicroWebStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
