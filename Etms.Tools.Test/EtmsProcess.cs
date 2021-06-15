using Autofac;
using ETMS.Authority;
using ETMS.Cache.Redis;
using ETMS.Cache.Redis.Wrapper;
using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using ETMS.Event.DataContract;
using ETMS.EventConsumer;
using ETMS.IBusiness.Wechart;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Etms.Tools.Test
{
    public class EtmsProcess
    {
        public EtmsProcess()
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

        private void InitRabbitMq(ContainerBuilder container, RabbitMqConfig config)
        {
            var busControl = new SubscriptionAdapt().PublishAt(config.Host, "EtmsConsumerQueue", config.UserName, config.Password, config.Vhost, config.PrefetchCount);
            var publisher = new EventPublisher(busControl);
            container.RegisterInstance(publisher).As<IEventPublisher>();
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

        public void ProcessRole()
        {
            TestLock();
        }

        private void TestLock()
        {
            var _distributedLockDAL = CustomServiceLocator.GetInstance<IDistributedLockDAL>();
            var _lockKey = new StudentEnrolmentToken(6);
            while (true)
            {
                if (_distributedLockDAL.LockTake(_lockKey))
                {
                    Console.WriteLine($"{DateTime.Now.EtmsToString()}获得执行锁,正在执行....");
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now.EtmsToString()}等待中...");
                    System.Threading.Thread.Sleep(1000 * 2);
                    _distributedLockDAL.LockRelease(_lockKey);
                }
            }
        }

        private const int _pageSize = 100;

        public void ProcessT()
        {
            var _sysTenantDAL = CustomServiceLocator.GetInstance<ISysTenantDAL>();
            var eventPublisher = CustomServiceLocator.GetInstance<IEventPublisher>();
            var pageCurrent = 1;
            var getTenantsEffectiveResult = _sysTenantDAL.GetTenantsEffective(_pageSize, pageCurrent).Result;
            if (getTenantsEffectiveResult.Item2 == 0)
            {
                return;
            }
            HandleTenantList(getTenantsEffectiveResult.Item1, eventPublisher);
            var totalPage = EtmsHelper.GetTotalPage(getTenantsEffectiveResult.Item2, _pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                getTenantsEffectiveResult = _sysTenantDAL.GetTenantsEffective(_pageSize, pageCurrent).Result;
                HandleTenantList(getTenantsEffectiveResult.Item1, eventPublisher);
                pageCurrent++;
            }
        }

        private void HandleTenantList(IEnumerable<SysTenant> tenantList, IEventPublisher eventPublisher)
        {
            if (tenantList == null || !tenantList.Any())
            {
                return;
            }
            foreach (var tenant in tenantList)
            {
                var now = new DateTime(2020, 01, 01);
                while (now < DateTime.Now)
                {
                    eventPublisher.Publish(new StatisticsStudentCountEvent(tenant.Id)
                    {
                        Time = now
                    });
                    now = now.AddMonths(1);
                }
            }
        }

        private void SetRole()
        {
            //var _sysTenantDAL = CustomServiceLocator.GetInstance<ISysTenantDAL>();
            //var tenantList = _sysTenantDAL.GetTenantsNormal().Result;
            //var _roleBll = CustomServiceLocator.GetInstance<IRoleDAL>();
            //foreach (var myTenant in tenantList)
            //{
            //    try
            //    {
            //        _roleBll.ResetTenantId(myTenant.Id);
            //        var allRole = _roleBll.GetRole().Result;
            //        if (allRole.Count > 0)
            //        {
            //            foreach (var myRole in allRole)
            //            {
            //                var roleAuthorityValueMenu = myRole.AuthorityValueMenu;
            //                var arrayRoleAuthorityValueMenu = roleAuthorityValueMenu.Split('|');
            //                var pageWeight = arrayRoleAuthorityValueMenu[2].ToBigInteger();
            //                var authorityCorePage = new AuthorityCore(pageWeight);
            //                if (authorityCorePage.Validation(26) && !authorityCorePage.Validation(90))
            //                {
            //                    authorityCorePage.RegisterAuthority(90);
            //                    authorityCorePage.RegisterAuthority(91);
            //                    var s = authorityCorePage.GetWeigth().ToString();
            //                    arrayRoleAuthorityValueMenu[2] = s;
            //                    myRole.AuthorityValueMenu = string.Join('|', arrayRoleAuthorityValueMenu);
            //                    //myRole.Name = $"{myRole.Name}_自动处理";
            //                    _roleBll.EditRole(myRole).Wait();
            //                    Console.WriteLine($"处理完成:{myRole.Name}  {myRole.AuthorityValueMenu}");
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"执行出错:{myTenant.Name}  {ex.Message}");
            //    }
            //}
        }
    }
}
