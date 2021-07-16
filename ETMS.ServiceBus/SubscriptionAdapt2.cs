using ETMS.EventConsumer;
using ETMS.IOC;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.ServiceBus
{
    public class SubscriptionAdapt2
    {
        /// <summary>
        /// 系统组建是否加载完毕    [已废弃此逻辑]
        /// 用于处理rabbitmq消费跑在了autofac注入前面，导致报错
        /// rabbitmq消费时判断系统初始化是否完毕，如果未完毕则等待1分钟执行消费
        /// </summary>
        public static bool IsSystemLoadingFinish = false;

        /// <summary>
        /// 服务总线
        /// </summary>
        public static IBusControl MassTransitBusService
        {
            get;
            private set;
        }

        #region 需要消费Event时 

        /// <summary>
        /// rabbitmq://host:port/vhost
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="vhost"></param>
        /// <param name="prefetchCount"></param>
        public void MassTransitInit(string host, string userName, string password, string vhost, ushort prefetchCount)
        {
            var uri = new Uri($"rabbitmq://{host}{vhost}");
            MassTransitBusService = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(uri, h =>
                {
                    h.Username(userName);
                    h.Password(password);
                });
                sbc.PrefetchCount = prefetchCount;
                //sbc.ConcurrentMessageLimit = 5; 
            });
        }

        public void MassTransitReceiveEndpoint<T>(string queue) where T : ETMS.Event.DataContract.Event
        {
            MassTransitBusService.ConnectReceiveEndpoint(queue, x =>
            {
                x.Handler<T>(msgBag =>
                {
                    //if (!IsSystemLoadingFinish)
                    //{
                    //    System.Threading.Thread.Sleep(600 * 1000);
                    //}
                    return CustomServiceLocator.GetInstance<IEventConsumer<T>>().Consume(msgBag.Message);
                });
            });
        }

        public void MassTransitStart()
        {
            MassTransitBusService.Start();
        }

        #endregion

        /// <summary>
        /// 需要发送Event时 
        /// 初始化服务总线，并且立即启动服务总线
        /// </summary>
        /// <param name="host">服务器IP</param>
        /// <param name="queue">消费queue</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="vhost"></param>
        /// <param name="prefetchCount"></param>
        public void MassTransitInitAndStart(string host, string queue, string userName, string password, string vhost, ushort prefetchCount)
        {
            var uri = new Uri($"rabbitmq://{host}{vhost}");
            MassTransitBusService = Bus.Factory.CreateUsingRabbitMq(sbc =>
           {
               sbc.Host(uri, queue, x =>
               {
                   x.Username(userName);
                   x.Password(password);
               });
               sbc.PrefetchCount = prefetchCount;
           });
            MassTransitBusService.Start();
        }
    }
}
