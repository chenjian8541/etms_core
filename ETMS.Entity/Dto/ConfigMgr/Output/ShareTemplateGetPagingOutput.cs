using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.ConfigMgr.Output
{
    public class ShareTemplateGetPagingOutput
    {
        public long Id { get; set; }

        /// <summary>
        /// <see cref="EmShareTemplateType"/>
        /// </summary>
        public byte Type { get; set; }

        public string TypeDesc { get; set; }

        /// <summary>
        /// <see cref="EmShareTemplateUseType"/>
        /// </summary>
        public int UseType { get; set; }

        public string UseTypeDesc { get; set; }

        public string ImgKeyUrl { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// <see cref="EmShareTemplateStatus"/>
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// <see cref="EmBool"/>
        /// </summary>
        public byte IsSystem { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
