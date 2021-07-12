using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class NoticeConfigGetOutput
    {
        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmNoticeConfigExType"/>
        /// </summary>
        public int ExType { get; set; }

        public List<NoticeConfigGetItem> MyItems { get; set; }
    }

    public class NoticeConfigGetItem {
        public long Key { get; set; }

        public string Label { get; set; }
    }
}
