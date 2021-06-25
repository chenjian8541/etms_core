using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class ActiveWxMessageDetailGetPagingOutput
    {
        /// <summary>
        /// 主表ID
        /// </summary>
        public long WxMessageId { get; set; }

        /// <summary>
        /// 学员
        /// </summary>
        public long StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentPhone { get; set; }

        /// <summary>
        /// 是否已读 <see cref="EmBool"/>
        /// </summary>
        public byte IsRead { get; set; }

        public string IsReadDesc { get; set; }

        /// <summary>
        /// 是否需要家长确认   <see cref="EmBool"/>
        /// </summary>
        public byte IsNeedConfirm { get; set; }

        /// <summary>
        /// 是否确认   <see cref="EmBool"/>
        /// </summary>
        public byte IsConfirm { get; set; }

        public string IsConfirmDesc { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        public DateTime? ConfirmOt { get; set; }
    }
}
