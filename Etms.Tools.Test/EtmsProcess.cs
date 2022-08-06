using Autofac;
using Com.Fubei.OpenApi.Sdk;
using ETMS.Authority;
using ETMS.Cache.Redis;
using ETMS.Cache.Redis.Wrapper;
using ETMS.Entity.CacheBucket.RedisLock;
using ETMS.Entity.Config;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Database.Source;
using ETMS.Entity.Dto.Educational.Request;
using ETMS.Entity.Dto.PaymentService.Request;
using ETMS.Entity.Dto.Student.Request;
using ETMS.Entity.Dto.User.Request;
using ETMS.Entity.Enum;
using ETMS.Entity.Pay.Lcsw.Dto;
using ETMS.Entity.Pay.Lcsw.Dto.Request.Response;
using ETMS.Entity.Temp.View;
using ETMS.Entity.View;
using ETMS.Event.DataContract;
using ETMS.Event.DataContract.Statistics;
using ETMS.EventConsumer;
using ETMS.IBusiness.EventConsumer;
using ETMS.IBusiness.IncrementLib;
using ETMS.IBusiness.Wechart;
using ETMS.ICache;
using ETMS.IDataAccess;
using ETMS.IDataAccess.EtmsManage;
using ETMS.IEventProvider;
using ETMS.IOC;
using ETMS.LOG;
using ETMS.Pay.Lcsw;
using ETMS.Pay.Suixing;
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
using System.Threading.Tasks;
using Config = ETMS.Pay.Lcsw.Config;
using Senparc.Weixin.WxOpen;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using ETMS.Business.Common;
using ETMS.IDataAccess.ShareTemplate;

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
                InitPayConfig(appSettings.PayConfig);
                InitAliyunOssConfig(appSettings.AliyunOssConfig);
                InitSenparcWeixin(appSettings);
            });
            SubscriptionAdapt2.IsSystemLoadingFinish = true;
            Log.Info("[服务]处理服务业务成功...", typeof(ServiceProvider));
            Console.WriteLine("[服务]处理服务业务成功...");
        }

        private void InitAliyunOssConfig(AliyunOssConfig config)
        {
            AliyunOssUtil.InitAliyunOssConfig(config.BucketName, config.AccessKeyId,
                config.AccessKeySecret, config.Endpoint, config.OssAccessUrlHttp,
                config.OssAccessUrlHttps, config.RootFolder);
            AliyunOssSTSUtil.InitAliyunSTSConfig(config.STSAccessKeyId, config.STSAccessKeySecret, config.STSRoleArn, config.STSEndpoint);
        }

        private void InitPayConfig(PayConfig payConfig)
        {
            ETMS.Pay.Lcsw.Config.InitConfig(payConfig.LcswConfig.ApiMpHostPay, payConfig.LcswConfig.ApiMpHostMerchant,
                payConfig.LcswConfig.InstNo, payConfig.LcswConfig.InstToken);
            var globalConfig = FubeiOpenApiGlobalConfig.Instance;
            globalConfig.Api_1_0 = payConfig.FubeiConfig.Api01;
            globalConfig.Api_2_0 = payConfig.FubeiConfig.Api02;

            var suixingConfig = payConfig.SuixingConfig;
            var wxPay = payConfig.WxPay;
            ETMS.Pay.Suixing.SuiXingConfig.InitConfig(suixingConfig.PrivateKeyPem, suixingConfig.PublicKeyPem, suixingConfig.OrgId,
               suixingConfig.MerchantInfoQuery, suixingConfig.JsapiScan, suixingConfig.TradeQuery,
               suixingConfig.Refund, suixingConfig.RefundQuery, wxPay.WxOfficialAccountAppid, wxPay.WxMiniProgramAppid);
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
            var miniProgramConfig = mySenparcSetting.SenparcWeixinSetting.MiniProgramConfig;
            var senparcWeixinSetting = new Senparc.Weixin.Entities.SenparcWeixinSetting()
            {
                Component_Appid = mySenparcSetting.SenparcWeixinSetting.ComponentConfig.ComponentAppid,
                Component_Secret = mySenparcSetting.SenparcWeixinSetting.ComponentConfig.ComponentSecret,
                Component_Token = mySenparcSetting.SenparcWeixinSetting.ComponentConfig.ComponentToken,
                Component_EncodingAESKey = mySenparcSetting.SenparcWeixinSetting.ComponentConfig.ComponentEncodingAESKey,
                WxOpenAppId = miniProgramConfig.WxOpenAppId,
                WxOpenAppSecret = miniProgramConfig.WxOpenAppSecret,
                WxOpenToken = miniProgramConfig.WxOpenToken,
                WxOpenEncodingAESKey = miniProgramConfig.WxOpenEncodingAESKey
            };
            var register = RegisterService.Start(senparcSetting);
            register.ChangeDefaultCacheNamespace("ETMSDefaultCacheNamespace");

            Senparc.CO2NET.Cache.CsRedis.Register.SetConfigurationOption(mySenparcSetting.SenparcSetting.CacheRedisConfiguration);
            Senparc.CO2NET.Cache.CsRedis.Register.UseKeyValueRedisNow();
            Console.WriteLine("[服务]完成 Redis 设置");
            Log.Info("[服务]完成 Redis 设置...", typeof(ServiceProvider));

            register.UseSenparcWeixin(senparcWeixinSetting, (weixinRegister, registerService) =>
            {
                weixinRegister.UseSenparcWeixinCacheCsRedis();
                weixinRegister.RegisterWxOpenAccount(senparcWeixinSetting, "【小禾帮培训管理系统】小程序");
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
                            ETMS.LOG.Log.Error($"[getComponentVerifyTicketFunc]componentAppId:{componentAppId}", ex, typeof(ServiceProvider));
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
                            ETMS.LOG.Log.Error($"[getAuthorizerRefreshTokenFunc]componentAppId:{componentAppId},auhtorizerId:{auhtorizerId}", ex, typeof(ServiceProvider));
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
                            ETMS.LOG.Log.Error($"[authorizerTokenRefreshedFunc]componentAppId:{componentAppId},auhtorizerId:{auhtorizerId}", ex, typeof(ServiceProvider));
                        }
                    }, "【小禾帮培训管理系统】开放平台");
            });

            Console.WriteLine($"[服务]前缓存策略: {CacheStrategyFactory.GetObjectCacheStrategyInstance()}");
            Log.Info("[服务]前缓存策略: {CacheStrategyFactory.GetObjectCacheStrategyInstance()}...", typeof(ServiceProvider));

            Log.Info("[服务]SenparcWeixin初始化完成...", typeof(ServiceProvider));
            Console.WriteLine("[服务]SenparcWeixin初始化完成");
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
        private IStudentDAL _studentDAL;
        private IAiface _aiface;
        private IShareTemplateIdDAL _shareTemplateIdDAL;
        private List<YearAndMonth> _yearAndMonths = new List<YearAndMonth>();
        public void ProcessT()
        {
            //var parentMenusConfigDAL = CustomServiceLocator.GetInstance<IParentMenusConfigDAL>();
            //for (var i = 1; i < 11470; i++)
            //{
            //    parentMenusConfigDAL.ResetTenantId(i);
            //    parentMenusConfigDAL.ClearMenuConfig();
            //    Console.WriteLine(i.ToString());
            //}
            //return;
            //var paySuixingService = CustomServiceLocator.GetInstance<IPaySuixingService>();
            //var a = paySuixingService.Refund(new ETMS.Pay.Suixing.Utility.ExternalDto.Request.RefundReq()
            //{
            //    amt = 0.01M,
            //    extend = "123",
            //    mno = "399220617483310",
            //    notifyUrl = "http://172.16.155.45:8080/order/test/call",
            //    origUuid = "123123",
            //    refundReason = "11",
            //    ordNo = OrderNumberLib.SuixingRefundOrder()
            //});
            //var bb = paySuixingService.MerchantInfoQuery("399200623916234");
            //var aa = paySuixingService.JsapiScanMiniProgram(new ETMS.Pay.Suixing.Utility.ExternalDto.Request.JsapiScanMiniProgramReq()
            //{
            //    mno = "399200623916234",
            //    notifyUrl = "http://172.16.155.45:8080/order/test/call",
            //    subject = "跆拳道课程",
            //    ordNo = "20112121111212",
            //    amt = 1.25M,
            //    openid = "2088100953812025",
            //     extend ="121222121"
            //});
            //var cc = paySuixingService.TradeQuery("399200623916234", "20112121111212");
            //var dd = paySuixingService.Refund(new ETMS.Pay.Suixing.Utility.ExternalDto.Request.RefundReq()
            //{
            //    amt = 1.25M,
            //    refundReason = "退货",
            //    mno = "399200623916234",
            //    ordNo = "TH32333232323232",
            //    notifyUrl = "http://172.16.155.45:8080/order/test/call",
            //    extend = "22222",
            //    origOrderNo = "20112121111212"
            //});

            //var ee = paySuixingService.RefundQuery("399200623916234", "TH32333232323232");
            //return;
            //var _cloudFileAutoDelDayBLL = CustomServiceLocator.GetInstance<ICloudFileAutoDelDayBLL>();
            //_cloudFileAutoDelDayBLL.InitTenantId(1);
            //_cloudFileAutoDelDayBLL.CloudFileAutoDelDayConsumerEvent(new CloudFileAutoDelDayEvent(1)
            //{
            //    DelDate = DateTime.Now,
            //    FileTag = EmTenantCloudStorageType.microWebConfig
            //});
            //return;
            //var firstDate = new DateTime(2020, 01, 01);
            //var maxDate = new DateTime(2021, 10, 01);
            //while (firstDate < maxDate)
            //{
            //    _yearAndMonths.Add(new YearAndMonth()
            //    {
            //        Month = firstDate.Month,
            //        Year = firstDate.Year
            //    });
            //    firstDate = firstDate.AddMonths(1);
            //}
            _sysTenantDAL = CustomServiceLocator.GetInstance<ISysTenantDAL>();
            _classDAL = CustomServiceLocator.GetInstance<IClassDAL>();
            _roleDAL = CustomServiceLocator.GetInstance<IRoleDAL>();
            _classRecordDAL = CustomServiceLocator.GetInstance<IClassRecordDAL>();
            _userDAL = CustomServiceLocator.GetInstance<IUserDAL>();
            _shareTemplateIdDAL = CustomServiceLocator.GetInstance<IShareTemplateIdDAL>();
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
                HandleTenantInitializ(tenant.Id);
            }
        }

        private void HandleTenantInitializ(int tenantId)
        {
            _shareTemplateIdDAL.InitTenantId(tenantId);
            _shareTemplateIdDAL.AddShareTemplate(new EtShareTemplate()
            {
                Type = EmShareTemplateType.Link,
                UseType = EmShareTemplateUseType.Achievement,
                Name = "默认成绩单分享链接模板",
                Title = "{{学员姓名}}同学的成绩单",
                Summary = "{{考试名称}}",
                ImgKey = "system/material/share/link/3.jpg",
                Status = EmShareTemplateStatus.Enabled,
                TenantId = tenantId,
                CreateTime = DateTime.Now,
                UpdateTime = null,
                IsDeleted = EmIsDeleted.Normal,
                IsSystem = EmBool.True,
                UserId = 0
            }).Wait();
        }

        private void ExecuteTeacherSalaryClassTimes(int tenantId)
        {
            _classRecordDAL.ResetTenantId(tenantId);
            ProcessClassRecord(tenantId);
        }

        private void ProcessStudent(int tenantId)
        {
            _aiface.InitTenantId(tenantId);
            _studentDAL.InitTenantId(tenantId);
            var query = new StudentGetPagingRequest()
            {
                LoginTenantId = tenantId,
                PageCurrent = 1,
                PageSize = 100,
                IsHasFaceKey = 1
            };
            var pagingData = _studentDAL.GetStudentPaging(query).Result;
            if (pagingData.Item2 == 0)
            {
                return;
            }
            HandleStudent(tenantId, pagingData.Item1);
            var totalPage = EtmsHelper.GetTotalPage(pagingData.Item2, _pageSize);
            query.PageCurrent++;
            while (query.PageCurrent <= totalPage)
            {
                pagingData = _studentDAL.GetStudentPaging(query).Result;
                HandleStudent(tenantId, pagingData.Item1);
                query.PageCurrent++;
            }
        }

        private void HandleStudent(int tenantId, IEnumerable<EtStudent> student)
        {
            foreach (var p in student)
            {
                var ss = AliyunOssUtil.GetAccessUrlHttps(p.FaceKey);
                var a = _aiface.StudentInitFace(p.Id, ss).Result;
                Console.WriteLine($"{p.Name}{a.Item2}:{ss}");
            }
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

        private void GetLcsPayInfo(string terminaId)
        {
            _sysTenantDAL = CustomServiceLocator.GetInstance<ISysTenantDAL>();
            var payLcswService = CustomServiceLocator.GetInstance<IPayLcswService>();
            var traceno = Guid.NewGuid().ToString("N");
            var ss = payLcswService.QueryTermina(traceno, terminaId);
        }

        private MerchantSaveRequest ReassignMerchantSaveRequestProperty(MerchantSaveRequest request, ResponseQuerMerchant queryOut)
        {
            request.account_name = queryOut.account_name;
            request.account_no = queryOut.account_no;
            request.account_phone = queryOut.account_phone;
            request.account_type = queryOut.account_type;
            request.artif_nm = queryOut.artif_nm;
            request.bank_name = queryOut.bank_name;
            request.bank_no = queryOut.bank_no;
            request.business_name = queryOut.business_name;
            request.business_code = queryOut.business_code;
            request.daily_timely_status = queryOut.daily_timely_status;
            request.img_3rd_part = queryOut.img_3rd_part;
            request.img_bankcard_a = queryOut.img_bankcard_a;
            request.img_bankcard_b = queryOut.img_bankcard_b;
            request.img_cashier = queryOut.img_cashier;
            request.img_contract = queryOut.img_contract;
            request.img_idcard_a = queryOut.img_idcard_a;
            request.img_idcard_b = queryOut.img_idcard_b;
            request.img_idcard_holding = queryOut.img_idcard_holding;
            request.img_indoor = queryOut.img_indoor;
            request.img_license = queryOut.img_license;
            request.img_logo = queryOut.img_logo;
            request.img_open_permits = queryOut.img_open_permits;
            request.img_org_code = queryOut.img_org_code;
            request.img_other = queryOut.img_other;
            request.img_private_idcard_a = queryOut.img_private_idcard_a;
            request.img_private_idcard_b = queryOut.img_private_idcard_b;
            request.img_standard_protocol = queryOut.img_standard_protocol;
            request.img_sub_account_promiss = queryOut.img_sub_account_promiss;
            request.img_tax_reg = queryOut.img_tax_reg;
            request.img_unincorporated = queryOut.img_unincorporated;
            request.img_val_add_protocol = queryOut.img_val_add_protocol;
            request.legalIdnum = queryOut.legalIdnum;
            request.legalIdnumExpire = queryOut.legalIdnumExpire;
            request.license_expire = queryOut.license_expire;
            request.license_no = queryOut.license_no;
            request.license_type = queryOut.license_type;
            request.merchant_address = queryOut.merchant_address;
            request.merchant_alias = queryOut.merchant_alias;
            request.merchant_business_type = queryOut.merchant_business_type;
            request.merchant_city = queryOut.merchant_city;
            request.merchant_city_code = queryOut.merchant_city_code;
            request.merchant_company = queryOut.merchant_company;
            request.merchant_county = queryOut.merchant_county;
            request.merchant_county_code = queryOut.merchant_county_code;
            request.merchant_email = queryOut.merchant_email;
            request.merchant_id_expire = queryOut.merchant_id_expire;
            request.merchant_id_no = queryOut.merchant_id_no;
            request.merchant_name = queryOut.merchant_name;
            request.merchant_no = queryOut.merchant_no;
            request.merchant_person = queryOut.merchant_person;
            request.merchant_phone = queryOut.merchant_phone;
            request.merchant_province = queryOut.merchant_province;
            request.merchant_province_code = queryOut.merchant_province_code;
            request.merchant_service_phone = queryOut.merchant_service_phone;
            request.rate_code = queryOut.rate_code;
            request.settlement_type = queryOut.settlement_type;
            request.settle_type = queryOut.settle_type;

            request.merchant_business_typeDesc = LcswEm.GetMerchantBusinessType(request.merchant_business_type);
            request.account_typeDesc = LcswEm.GetAccountTypeDesc(request.account_type);
            request.settlement_typeDesc = LcswEm.GetSettlementType(request.settlement_type);
            return request;
        }

        private static LcswStatusView GetLcswStatus(string merchant_status)
        {
            var lcswApplyStatus = merchant_status.ToInt();
            var lcswOpenStatus = EmBool.False;
            if (MerchantStatus.IsPass(merchant_status))
            {
                lcswOpenStatus = EmBool.True;
            }
            return new LcswStatusView()
            {
                LcswApplyStatus = lcswApplyStatus,
                LcswOpenStatus = lcswOpenStatus
            };
        }

        /// <summary>
        /// 绑定扫呗账户
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="merchantNo"></param>
        /// <param name="terminaId"></param>
        /// <returns></returns>
        private async Task<string> BindLcsPay(int tenantId, string merchantNo, string terminaId)
        {
            _sysTenantDAL = CustomServiceLocator.GetInstance<ISysTenantDAL>();
            var payLcswService = CustomServiceLocator.GetInstance<IPayLcswService>();
            var _tenantLcsAccountDAL = CustomServiceLocator.GetInstance<ITenantLcsAccountDAL>();
            var addRes = payLcswService.QuerMerchant(merchantNo);
            if (!addRes.IsSuccess())
            {
                return "扫呗账户信息异常";
            }
            var myTenant = await _sysTenantDAL.GetTenant(tenantId);
            if (myTenant == null)
            {
                return "机构不存在";
            }
            var request = new MerchantSaveRequest();
            request = ReassignMerchantSaveRequestProperty(request, addRes);
            var traceno = Guid.NewGuid().ToString("N");
            var addTerminalRes = payLcswService.QueryTermina(traceno, terminaId);
            var now = DateTime.Now;
            var lcswStatus = GetLcswStatus(addRes.merchant_status);
            await _tenantLcsAccountDAL.AddTenantLcsAccount(new SysTenantLcsAccount()
            {
                AgentId = myTenant.AgentId,
                ChangeTime = now,
                CreationTime = now,
                InstNo = Config._instNo,
                IsDeleted = EmIsDeleted.Normal,
                LcswApplyStatus = lcswStatus.LcswApplyStatus,
                MerchantCompany = request.merchant_company,
                MerchantName = request.merchant_name,
                MerchantNo = merchantNo,
                MerchantStatus = addRes.merchant_status,
                MerchantType = addRes.merchant_type,
                Remark = string.Empty,
                ResultCode = addRes.result_code,
                ReturnCode = addRes.return_code,
                ReturnMsg = addRes.return_msg,
                ReviewTime = null,
                StoreCode = string.Empty,
                TenantId = myTenant.Id,
                TerminalId = addTerminalRes.terminal_id,
                TerminalName = string.Empty,
                AccessToken = addTerminalRes.access_token,
                TraceNo = string.Empty,
                MerchantInfoData = Newtonsoft.Json.JsonConvert.SerializeObject(addRes),
                MerchantRquestData = Newtonsoft.Json.JsonConvert.SerializeObject(request)
            });
            await _sysTenantDAL.UpdateTenantLcswInfo(myTenant.Id, lcswStatus.LcswApplyStatus, lcswStatus.LcswOpenStatus);
            return string.Empty;
        }
    }
}
