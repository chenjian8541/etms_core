using ETMS.Entity.Database.Manage;
using ETMS.Entity.View;

namespace ETMS.Event.DataContract
{
    public class NoticeManageEvent : Event
    {
        /// <summary>
        /// <see cref="NoticeManageType"/>
        /// </summary>
        public int Type { get; set; }

        public SysTryApplyLog TryApplyLog { get; set; }

        public DangerousVisitor MyDangerousVisitor { get; set; }
    }

    public struct NoticeManageType
    {
        public const int TryApply = 0;

        public const int DangerousIp = 1;
    }
}
