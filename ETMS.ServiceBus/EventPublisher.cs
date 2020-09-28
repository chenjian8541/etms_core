﻿using MassTransit;
using ETMS.IEventProvider;

namespace ETMS.ServiceBus
{
    /// <summary>
    /// 消息发布者
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        /// <summary>
        /// rabbitmq企业服务总线
        /// </summary>
        private readonly IBusControl _bus;

        /// <summary>
        /// 构造函数
        /// 初始化rabbitmq企业服务总线
        /// </summary>
        /// <param name="bus"></param>
        public EventPublisher(IBusControl bus)
        {
            this._bus = bus;
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void Publish<T>(T message) where T : ETMS.Event.DataContract.Event
        {
            _bus.Publish(message);
        }
    }
}
