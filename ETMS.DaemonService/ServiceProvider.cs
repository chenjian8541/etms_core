using Autofac;
using ETMS.Cache.Redis;
using ETMS.Cache.Redis.Wrapper;
using ETMS.Entity.Config;
using ETMS.Event.DataContract;
using ETMS.EventConsumer;
using ETMS.ICache;
using ETMS.IEventProvider;
using ETMS.IOC;
using ETMS.LOG;
using ETMS.ServiceBus;
using ETMS.Utility;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ETMS.DaemonService
{
    /// <summary>
    /// 服务处理类
    /// </summary>
    public class ServiceProvider
    {
        /// <summary>
        /// 处理服务业务
        /// </summary>
        public static void Process()
        {
            AppSettings appSettings = null;
            Bootstrapper.Bootstrap(p =>
            {
                appSettings = InitCustomIoc(p);
                InitRabbitMq(p, appSettings.RabbitMqConfig);
            });
            SubscriptionAdapt.IsSystemLoadingFinish = true;
            Log.Info("[服务]处理服务业务成功...", typeof(ServiceProvider));
            Console.WriteLine("[服务]处理服务业务成功...");
        }

        /// <summary>
        /// 自定义一些注入规则
        /// </summary>
        /// <param name="container"></param>
        private static AppSettings InitCustomIoc(ContainerBuilder container)
        {
            //appsettings
            var appsettingsJson = File.ReadAllText(FileHelper.GetFilePath("appsettings.json"));
            var appSettings = JsonConvert.DeserializeObject<AppSettings>(appsettingsJson);
            var appConfigurtaionServices = new AppConfigurtaionServices(null) { AppSettings = appSettings };
            container.RegisterInstance(appConfigurtaionServices).As<IAppConfigurtaionServices>();
            //RedisConfig
            CSRedisWrapper.Initialize(appSettings.RedisConfig.ServerConStrFormat, appSettings.RedisConfig.DbCount);
            container.RegisterType<RedisProvider>().As<ICacheProvider>();
            //IHttpClient
            container.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            container.RegisterType<StandardHttpClient>().As<IHttpClient>().SingleInstance();
            return appSettings;
        }

        /// <summary>
        /// 初始化rabbitmq
        /// </summary>
        /// <param name="container"></param>
        /// <param name="config"></param>
        private static void InitRabbitMq(ContainerBuilder container, RabbitMqConfig config)
        {
            var subscriptionAdapt = new SubscriptionAdapt();
            var consumers = Assembly.Load("ETMS.EventConsumer").GetTypes();
            foreach (var consumer in consumers)
            {
                var attributions = consumer.GetCustomAttributes(typeof(QueueConsumerAttribution), false);
                if (attributions.Length > 0)
                {
                    var consumerQueue = ((QueueConsumerAttribution)attributions[0]).QueueName;
                    var consumerSuperClass = consumer.GetInterfaces().FirstOrDefault(d => d.IsGenericType && d.GetGenericTypeDefinition() == typeof(IEventConsumer<>));
                    if (consumerSuperClass == null)
                    {
                        continue;
                    }
                    container.RegisterType(consumer).As(consumerSuperClass);
                    var consumerType = consumerSuperClass.GetGenericArguments().Single();
                    var methodInfo = typeof(SubscriptionAdapt).GetMethod("SubscribeAt").MakeGenericMethod(new Type[] { consumerType });
                    var busControl = (IBusControl)methodInfo.Invoke(subscriptionAdapt, new object[] { config.Host, consumerQueue, config.UserName, config.Password, config.Vhost, config.PrefetchCount });
                    var publisher = new EventPublisher(busControl);
                    container.RegisterInstance(publisher).As<IEventPublisher>();
                }
            }
            Log.Info("[服务]RabbitMq订阅成功...", typeof(ServiceProvider));
            Console.WriteLine("[服务]RabbitMq订阅成功");
        }
    }
}
