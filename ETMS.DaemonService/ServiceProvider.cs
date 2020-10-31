using Autofac;
using ETMS.Cache.Redis;
using ETMS.Cache.Redis.Wrapper;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.EventConsumer;
using ETMS.IBusiness.Wechart;
using ETMS.ICache;
using ETMS.IEventProvider;
using ETMS.IOC;
using ETMS.LOG;
using ETMS.ServiceBus;
using ETMS.Utility;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Senparc.CO2NET;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Cache.CsRedis;
using Senparc.Weixin.Open;
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
                InitSenparcWeixin(appSettings);
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
            var appSettings = JsonConvert.DeserializeObject<MyAppSettings>(appsettingsJson).AppSettings;
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

        /// <summary>
        /// 初始化SenparcWeixin
        /// </summary>
        /// <param name="appSettings"></param>
        private static void InitSenparcWeixin(AppSettings appSettings)
        {
            Console.WriteLine("[服务]开始初始化SenparcWeixin");
            Log.Info("[服务]开始初始化SenparcWeixin...", typeof(ServiceProvider));
            var services = new ServiceCollection();
            services.AddMemoryCache();

            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile(FileHelper.GetFilePath("appsettings.json"), false, false);
            var config = configBuilder.Build();
            services.AddSenparcGlobalServices(config);
            Console.WriteLine("[服务]完成 AddSenparcGlobalServices 注册");
            Log.Info("[服务]完成 AddSenparcGlobalServices 注册...", typeof(ServiceProvider));

            var mySenparcSetting = appSettings.SenparcConfig;
            var senparcSetting = new Senparc.CO2NET.SenparcSetting()
            {
                Cache_Memcached_Configuration = string.Empty,
                Cache_Redis_Configuration = mySenparcSetting.SenparcSetting.CacheRedisConfiguration,
                DefaultCacheNamespace = mySenparcSetting.SenparcSetting.DefaultCacheNamespace,
                IsDebug = mySenparcSetting.SenparcSetting.IsDebug,
                SenparcUnionAgentKey = mySenparcSetting.SenparcSetting.SenparcUnionAgentKey
            };
            var senparcWeixinSetting = new Senparc.Weixin.Entities.SenparcWeixinSetting()
            {
                Component_Appid = mySenparcSetting.SenparcWeixinSetting.ComponentConfig.ComponentAppid,
                Component_Secret = mySenparcSetting.SenparcWeixinSetting.ComponentConfig.ComponentSecret,
                Component_Token = mySenparcSetting.SenparcWeixinSetting.ComponentConfig.ComponentToken,
                Component_EncodingAESKey = mySenparcSetting.SenparcWeixinSetting.ComponentConfig.ComponentEncodingAESKey
            };
            var register = RegisterService.Start(senparcSetting);
            register.ChangeDefaultCacheNamespace("ETMSDefaultCacheNamespace");

            Senparc.CO2NET.Cache.CsRedis.Register.SetConfigurationOption(mySenparcSetting.SenparcSetting.CacheRedisConfiguration);
            Senparc.CO2NET.Cache.CsRedis.Register.UseKeyValueRedisNow();
            Console.WriteLine("[服务]完成 Redis 设置");
            Log.Info("[服务]完成 Redis 设置...", typeof(ServiceProvider));

            register.UseSenparcWeixin(senparcWeixinSetting, weixinRegister =>
            {
                weixinRegister.UseSenparcWeixinCacheCsRedis();
                weixinRegister.RegisterOpenComponent(senparcWeixinSetting,
                    async componentAppId =>
                    {
                        //getComponentVerifyTicketFunc 
                        try
                        {
                            var componentAccessBLL = CustomServiceLocator.GetInstance<IComponentAccessBLL>();
                            return await componentAccessBLL.GetSysWechartVerifyTicket(componentAppId);
                        }
                        catch (Exception ex)
                        {
                            LOG.Log.Error($"[getComponentVerifyTicketFunc]componentAppId:{componentAppId}", ex, typeof(ServiceProvider));
                            return string.Empty;
                        }
                    },
                    async (componentAppId, auhtorizerId) =>
                    {
                        //getAuthorizerRefreshTokenFunc
                        try
                        {
                            var componentAccessBLL = CustomServiceLocator.GetInstance<IComponentAccessBLL>();
                            var wechartAuthorizerToken = await componentAccessBLL.GetSysWechartAuthorizerToken(auhtorizerId);
                            if (wechartAuthorizerToken == null)
                            {
                                return null;
                            }
                            return wechartAuthorizerToken.AuthorizerRefreshToken;
                        }
                        catch (Exception ex)
                        {
                            LOG.Log.Error($"[getAuthorizerRefreshTokenFunc]componentAppId:{componentAppId},auhtorizerId:{auhtorizerId}", ex, typeof(ServiceProvider));
                            return string.Empty;
                        }
                    },
                    async (componentAppId, auhtorizerId, refreshResult) =>
                    {
                        //authorizerTokenRefreshedFunc
                        try
                        {
                            var componentAccessBLL = CustomServiceLocator.GetInstance<IComponentAccessBLL>();
                            var wechartAuthorizerToken = await componentAccessBLL.GetSysWechartAuthorizerToken(auhtorizerId);
                            if (wechartAuthorizerToken == null)
                            {
                                wechartAuthorizerToken = new SysWechartAuthorizerToken()
                                {
                                    IsDeleted = EmIsDeleted.Normal,
                                    ModifyOt = DateTime.Now,
                                    Remark = string.Empty,
                                    ComponentAppId = componentAppId,
                                    AuthorizerAppid = auhtorizerId,
                                    AuthorizerAccessToken = refreshResult.authorizer_access_token,
                                    AuthorizerRefreshToken = refreshResult.authorizer_refresh_token,
                                    ExpiresIn = refreshResult.expires_in
                                };
                            }
                            else
                            {
                                wechartAuthorizerToken.AuthorizerAccessToken = refreshResult.authorizer_access_token;
                                wechartAuthorizerToken.AuthorizerRefreshToken = refreshResult.authorizer_refresh_token;
                                wechartAuthorizerToken.ExpiresIn = refreshResult.expires_in;
                                wechartAuthorizerToken.ModifyOt = DateTime.Now;
                            }
                            await componentAccessBLL.SaveSysWechartAuthorizerToken(wechartAuthorizerToken);
                        }
                        catch (Exception ex)
                        {
                            LOG.Log.Error($"[authorizerTokenRefreshedFunc]componentAppId:{componentAppId},auhtorizerId:{auhtorizerId}", ex, typeof(ServiceProvider));
                        }
                    }, "【小禾帮培训管理系统】开放平台");
            });

            Console.WriteLine($"[服务]前缓存策略: {CacheStrategyFactory.GetObjectCacheStrategyInstance()}");
            Log.Info("[服务]前缓存策略: {CacheStrategyFactory.GetObjectCacheStrategyInstance()}...", typeof(ServiceProvider));

            Log.Info("[服务]SenparcWeixin初始化完成...", typeof(ServiceProvider));
            Console.WriteLine("[服务]SenparcWeixin初始化完成");
        }
    }
}
