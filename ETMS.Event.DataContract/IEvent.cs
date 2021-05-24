using System;

namespace ETMS.Event.DataContract
{
    /// <summary>
    /// 规范事件消息的格式
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        int TenantId { get; set; }

        /// <summary>
        /// 事件唯一码
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// 发送事件的服务器名
        /// </summary>
        string FromApplicationMachine { get; }

        /// <summary>
        /// 失败后尝试的次数
        /// </summary>
        int TryCount { get; set; }
    }
}
