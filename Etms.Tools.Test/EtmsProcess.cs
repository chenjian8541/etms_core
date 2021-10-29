using Autofac;
using ETMS.Authority;
using ETMS.Cache.Redis;
using ETMS.Cache.Redis.Wrapper;
using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Dto.User.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
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
            SubscriptionAdapt2.IsSystemLoadingFinish = true;
            Log.Info("[服务]处理服务业务成功...", typeof(ServiceProvider));
            Console.WriteLine("[服务]处理服务业务成功...");
        }

        private void InitRabbitMq(ContainerBuilder container, RabbitMqConfig config)
        {
            new SubscriptionAdapt2().MassTransitInitAndStart(config.Host, "EtmsConsumerQueue", config.UserName, config.Password, config.Vhost, config.PrefetchCount);
            var publisher = new EventPublisher();
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

        private IClassDAL _classDAL;
        private ISysTenantDAL _sysTenantDAL;
        private IEventPublisher _eventPublisher;
        private IRoleDAL _roleDAL;
        private IClassRecordDAL _classRecordDAL;
        private IUserDAL _userDAL;
        private List<YearAndMonth> _yearAndMonths = new List<YearAndMonth>();
        public void ProcessT()
        {
            var firstDate = new DateTime(2020, 01, 01);
            var maxDate = new DateTime(2021, 10, 01);
            while (firstDate < maxDate)
            {
                _yearAndMonths.Add(new YearAndMonth()
                {
                    Month = firstDate.Month,
                    Year = firstDate.Year
                });
                firstDate = firstDate.AddMonths(1);
            }
            _sysTenantDAL = CustomServiceLocator.GetInstance<ISysTenantDAL>();
            _classDAL = CustomServiceLocator.GetInstance<IClassDAL>();
            _roleDAL = CustomServiceLocator.GetInstance<IRoleDAL>();
            _classRecordDAL = CustomServiceLocator.GetInstance<IClassRecordDAL>();
            _userDAL = CustomServiceLocator.GetInstance<IUserDAL>();
            _eventPublisher = CustomServiceLocator.GetInstance<IEventPublisher>();
            var pageCurrent = 1;
            var getTenantsEffectiveResult = _sysTenantDAL.GetTenantsEffective(_pageSize, pageCurrent).Result;
            if (getTenantsEffectiveResult.Item2 == 0)
            {
                return;
            }
            HandleTenantList(getTenantsEffectiveResult.Item1);
            var totalPage = EtmsHelper.GetTotalPage(getTenantsEffectiveResult.Item2, _pageSize);
            pageCurrent++;
            while (pageCurrent <= totalPage)
            {
                getTenantsEffectiveResult = _sysTenantDAL.GetTenantsEffective(_pageSize, pageCurrent).Result;
                HandleTenantList(getTenantsEffectiveResult.Item1);
                pageCurrent++;
            }
        }

        private void HandleTenantList(IEnumerable<SysTenant> tenantList)
        {
            if (tenantList == null || !tenantList.Any())
            {
                return;
            }
            foreach (var tenant in tenantList)
            {
                ExecuteTeacherSalaryClassTimes(tenant.Id);
            }
        }

        private void ExecuteTeacherSalaryClassTimes(int tenantId)
        {
            _classRecordDAL.ResetTenantId(tenantId);
            ProcessClassRecord(tenantId);
        }

        private void ProcessUser(int tenantId)
        {
            var query = new UserGetPagingRequest()
            {
                LoginTenantId = tenantId,
                PageCurrent = 1,
                PageSize = 50
            };
            var pagingData = _userDAL.GetUserPaging(query).Result;
            if (pagingData.Item2 == 0)
            {
                return;
            }
            HandleUser(tenantId, pagingData.Item1);
            var totalPage = EtmsHelper.GetTotalPage(pagingData.Item2, _pageSize);
            query.PageCurrent++;
            while (query.PageCurrent <= totalPage)
            {
                pagingData = _userDAL.GetUserPaging(query).Result;
                HandleUser(tenantId, pagingData.Item1);
                query.PageCurrent++;
            }
        }

        private void HandleUser(int tenantId, IEnumerable<UserView> userList)
        {
            if (userList == null || !userList.Any())
            {
                return;
            }
            var ids = userList.Select(p => p.Id).ToList();
            _eventPublisher.Publish(new SyncTeacherMonthClassTimesEvent(tenantId)
            {
                TeacherIds = ids,
                YearAndMonthList = _yearAndMonths
            });
            Console.WriteLine($"{tenantId}处理中...");
        }


        private void ProcessClassRecord(int tenantId)
        {
            var query = new ClassRecordGetPagingRequest()
            {
                LoginTenantId = tenantId,
                PageCurrent = 1,
                PageSize = 200,
                Status = EmClassRecordStatus.Normal
            };
            var pagingData = _classRecordDAL.GetPaging(query).Result;
            if (pagingData.Item2 == 0)
            {
                return;
            }
            HandleClassRecord(pagingData.Item1);
            var totalPage = EtmsHelper.GetTotalPage(pagingData.Item2, _pageSize);
            query.PageCurrent++;
            while (query.PageCurrent <= totalPage)
            {
                pagingData = _classRecordDAL.GetPaging(query).Result;
                HandleClassRecord(pagingData.Item1);
                query.PageCurrent++;
            }
        }

        private void HandleClassRecord(IEnumerable<EtClassRecord> classRecordList)
        {
            if (classRecordList == null || !classRecordList.Any())
            {
                return;
            }
            foreach (var p in classRecordList)
            {
                _eventPublisher.Publish(new StatisticsTeacherSalaryClassTimesEvent(p.TenantId)
                {
                    ClassRecordId = p.Id
                });
                Console.WriteLine($"处理完成_上课记录:{p.CheckOt} ");
            }
        }


        //private void ProcessClass(int tenantId)
        //{
        //    var query = new ClassGetPagingRequest()
        //    {
        //        LoginTenantId = tenantId,
        //        PageCurrent = 1,
        //        PageSize = 200
        //    };
        //    var pagingData = _classDAL.GetPaging(query).Result;
        //    if (pagingData.Item2 == 0)
        //    {
        //        return;
        //    }
        //    HandleClass(pagingData.Item1);
        //    var totalPage = EtmsHelper.GetTotalPage(pagingData.Item2, _pageSize);
        //    query.PageCurrent++;
        //    while (query.PageCurrent <= totalPage)
        //    {
        //        pagingData = _classDAL.GetPaging(query).Result;
        //        HandleClass(pagingData.Item1);
        //        query.PageCurrent++;
        //    }
        //}

        //private void HandleClass(IEnumerable<EtClass> classList)
        //{
        //    if (classList == null || !classList.Any())
        //    {
        //        return;
        //    }
        //    foreach (var p in classList)
        //    {
        //        _eventPublisher.Publish(new StatisticsClassFinishCountEvent(p.TenantId)
        //        {
        //            ClassId = p.Id
        //        });
        //        Console.WriteLine($"处理完成:{p.Name} ");
        //    }
        //}


        //private void SetRole(int tenantId)
        //{
        //    try
        //    {
        //        var allRole = _roleDAL.GetRole().Result;
        //        if (allRole.Count > 0)
        //        {
        //            foreach (var myRole in allRole)
        //            {
        //                if (!string.IsNullOrEmpty(myRole.NoticeSetting))
        //                {
        //                    myRole.NoticeSetting = $"{myRole.NoticeSetting}7,8,";
        //                    _roleDAL.EditRole(myRole).Wait();
        //                    Console.WriteLine($"处理完成:{myRole.Name} ");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"{ex.Message}");
        //    }
        //}

        public void EventPublish()
        {
            _eventPublisher = CustomServiceLocator.GetInstance<IEventPublisher>();
            _eventPublisher.Publish(new StatisticsEducationEvent(5412)
            {
                Time = new DateTime(2021, 7, 30)
            });
        }
    }
}
