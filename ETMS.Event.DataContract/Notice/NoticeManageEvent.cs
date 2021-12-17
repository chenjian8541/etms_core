using ETMS.Entity.Database.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Event.DataContract
{
    public class NoticeManageEvent : Event
    {
        /// <summary>
        /// <see cref="NoticeManageType"/>
        /// </summary>
        public int Type { get; set; }

        public SysTryApplyLog TryApplyLog { get; set; }
    }

    public struct NoticeManageType
    {
        public const int TryApply = 0;
    }
}
