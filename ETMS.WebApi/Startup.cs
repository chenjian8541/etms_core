using Autofac;
using Autofac.Extensions.DependencyInjection;
using ETMS.Entity.Config;
using ETMS.IEventProvider;
using ETMS.IOC;
using ETMS.ServiceBus;
using ETMS.Utility;
using ETMS.WebApi.Extensions;
using ETMS.WebApi.FilterAttribute;
using ETMS.WebApi.Lib.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Senparc.CO2NET;
using System;
using System.Text;
using Senparc.CO2NET.AspNet;
using Senparc.Weixin;
using Senparc.Weixin.Cache.CsRedis;
using Senparc.Weixin.Open;
using ETMS.IBusiness.Wechart;
using ETMS.Entity.Database.Manage;
using ETMS.Entity.Enum;
using Senparc.Weixin.RegisterServices;
using AspNetCoreRateLimit;
using ETMS.WebApi.Core;

namespace ETMS.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string PolicyName = "EtmsDomainLimit";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddSession();
            services.AddResponseCompression();
            services.AddResponseCaching();
            services.AddJwtAuthentication();
            services.AddMemoryCache();
            var appSettings = appSettingsSection.Get<AppSettings>();
            services.AddRedis(appSettings.RedisConfig);
            InitRateLimit(services, appSettings.RedisConfig);
            services.AddMvc(op =>
            {
                RegisterGlobalFilters(op.Filters);
            }).AddNewtonsoftJson(jsonOptions =>
            {
                jsonOptions.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm";
                jsonOptions.SerializerSettings.ContractResolver = new EtmsContractResolver()
                {
                    NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
                };
            });
            services.AddOptions();
            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName, builder => builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
            });
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            services.AddSenparcWeixinServices(Configuration);
            InitCustomIoc(services);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();
            InitRabbitMq(builder, appSettings.RabbitMqConfig);
            InitAliyunOssConfig(appSettings.AliyunOssConfig);
        }

        private void RegisterGlobalFilters(FilterCollection filters)
        {
            filters.Add(new EtmsValidateRequestAttribute());
            filters.Add(new EtmsExceptionFilterAttribute());
            filters.Add(new EtmsResponseCacheAttribute());
        }

        private void InitCustomIoc(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpClient, StandardHttpClient>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        private void InitRateLimit(IServiceCollection services, RedisConfig redisConfig)
        {
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisConfig.ServerConStrDefault;
                options.InstanceName = "ETMSRatelimit";
            });
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
        }

        private void InitRabbitMq(ContainerBuilder container, RabbitMqConfig config)
        {
            var busControl = new SubscriptionAdapt().PublishAt(config.Host, "EtmsConsumerQueue", config.UserName, config.Password, config.Vhost, config.PrefetchCount);
            var publisher = new EventPublisher(busControl);
            container.RegisterInstance(publisher).As<IEventPublisher>();
        }

        private void InitAliyunOssConfig(AliyunOssConfig config)
        {
            AliyunOssUtil.InitAliyunOssConfig(config.BucketName, config.AccessKeyId,
                config.AccessKeySecret, config.Endpoint, config.OssAccessUrlHttp,
                config.OssAccessUrlHttps, config.RootFolder);
            AliyunOssUtil.SetBucketLifecycle(AliyunOssTempFileTypeEnum.FaceBlacklist, 2);
            AliyunOssUtil.SetBucketLifecycle(AliyunOssTempFileTypeEnum.FaceStudentCheckOn, 7);
            AliyunOssSTSUtil.InitAliyunSTSConfig(config.STSAccessKeyId, config.STSAccessKeySecret, config.STSRoleArn, config.STSEndpoint);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();
            app.UseResponseCompression();
            app.UseResponseCaching();
            app.UseMiddleware<EtmsIpRateLimitMiddleware>();
            app.UseCors(PolicyName);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            var appConfig = CustomServiceLocator.GetInstance<IAppConfigurtaionServices>().AppSettings;
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(appConfig.StaticFilesConfig.ServerPath),
                RequestPath = new PathString(appConfig.StaticFilesConfig.VirtualPath),
                EnableDirectoryBrowsing = false
            });

            this.InitSenparcService(app, env, appConfig);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void InitSenparcService(IApplicationBuilder app, IWebHostEnvironment env, AppSettings appSettings)
        {
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
            var registerService = app.UseSenparcGlobal(env, senparcSetting, globalRegister =>
            {
                globalRegister.ChangeDefaultCacheNamespace("ETMSDefaultCacheNamespace");
                Senparc.CO2NET.Cache.CsRedis.Register.SetConfigurationOption(mySenparcSetting.SenparcSetting.CacheRedisConfiguration);
                Senparc.CO2NET.Cache.CsRedis.Register.UseKeyValueRedisNow();
            }, true).UseSenparcWeixin(senparcWeixinSetting, weixinRegister =>
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
                            LOG.Log.Error($"[getComponentVerifyTicketFunc]componentAppId:{componentAppId}", ex, this.GetType());
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
                            LOG.Log.Error($"[getAuthorizerRefreshTokenFunc]componentAppId:{componentAppId},auhtorizerId:{auhtorizerId}", ex, this.GetType());
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
                            LOG.Log.Error($"[authorizerTokenRefreshedFunc]componentAppId:{componentAppId},auhtorizerId:{auhtorizerId}", ex, this.GetType());
                        }
                    }, "【小禾帮培训管理系统】开放平台");
            });
        }
    }
}
