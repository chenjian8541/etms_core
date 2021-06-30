using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Event.DataContract
{
    /// <summary>
    /// 事件消息
    /// </summary>
    public class Event : IEvent
    {
        /// <summary>
        /// 默认程序名称
        /// </summary>
        public const string DefaultApplicationName = "ETMS";

        /// <summary>
        /// 发送者的服务器名称
        /// </summary>
        public static readonly string DefaultMachineName = Environment.MachineName;

        public Event()
        {
        }

        /// <summary>
        /// 构造函数
        /// 初始化事件默认信息
        /// </summary>
        protected Event(int tenantId)
        {
            Id = Guid.NewGuid();
            CreateTime = DateTime.Now;
            FromApplicationMachine = DefaultApplicationName;
            this.TenantId = tenantId;
        }

        /// <summary>
        /// 机构ID
        /// </summary>
        public int TenantId { get; set; }

        public long UserId { get; set; }

        /// <summary>
        /// 事件唯一码
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 发送事件的服务器名
        /// </summary>
        public string FromApplicationMachine { get; set; }

        /// <summary>
        /// 客户端类型  <see cref="ETMS.Entity.Enum.EmUserOperationLogClientType"/>
        /// </summary>
        public int LoginClientType { get; set; }

        /// <summary>
        /// 失败后尝试的次数
        /// </summary>
        public int TryCount { get; set; }

        /// <summary>
        /// 错误情况下 尝试的次数
        /// </summary>
        public int ErrTryCount { get; set; }
    }
}
