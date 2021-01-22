using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.EtmsManage.Dto.Explain.Output
{
    public class SysExplainPagingOutput
    {
        public int Id { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmSysExplainType"/>
        /// </summary>
        public int Type { get; set; }

        public string Title { get; set; }

        public string RelationUrl { get; set; }

        public string TypeDesc { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
