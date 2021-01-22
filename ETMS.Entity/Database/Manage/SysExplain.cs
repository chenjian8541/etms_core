using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ETMS.Entity.Database.Manage
{
    /// <summary>
    /// 系统说明
    /// </summary>
    [Table("SysExplain")]
    public class SysExplain: EManageEntity<int>
    {
        public int AgentId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmSysExplainType"/>
        /// </summary>
        public int Type { get; set; }

        public string Title { get; set; }

        public string RelationUrl { get; set; }

        public DateTime Ot { get; set; }
    }
}
