using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.EtmsManage.Dto.Agent3.Output
{
    public class SysSysNoticeBulletinPagingOutput
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public string EndTime { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte IsAdvertise { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmNoticeBulletinStatus"/>
        /// </summary>
        public byte Status { get; set; }
    }
}
