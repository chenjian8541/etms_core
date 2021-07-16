using MassTransit;
using ETMS.IEventProvider;

namespace ETMS.ServiceBus
{
    /// <summary>
    /// 消息发布者
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void Publish<T>(T message) where T : ETMS.Event.DataContract.Event
        {
            SubscriptionAdapt2.MassTransitBusService.Publish(message);
        }
    }
}
