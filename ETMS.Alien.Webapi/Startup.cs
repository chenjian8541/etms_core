using Autofac;
using ETMS.Alien.Webapi.Extensions;
using ETMS.Alien.Webapi.FilterAttribute;
using ETMS.Alien.Webapi.Lib.Json;
using ETMS.Entity.Config;
using ETMS.IEventProvider;
using ETMS.IOC;
using ETMS.ServiceBus;
using ETMS.Utility;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.FileProviders;
using System.Text;

namespace ETMS.Alien.Webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string PolicyName = "EtmsAlienDomainLimit";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddSession();
            services.AddResponseCompression();
            services.AddResponseCaching();
            services.AddJwtAuthentication();
            services.AddRedis(appSettingsSection.Get<AppSettings>().RedisConfig);
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
        }

        private void InitRabbitMq(ContainerBuilder container, RabbitMqConfig config)
        {
            new SubscriptionAdapt2().MassTransitInitAndStart(config.Host, "EtmsConsumerQueue", config.UserName, config.Password, config.Vhost, config.PrefetchCount);
            var publisher = new EventPublisher();
            container.RegisterInstance(publisher).As<IEventPublisher>();
        }

        private void InitAliyunOssConfig(AliyunOssConfig config)
        {
            AliyunOssUtil.InitAliyunOssConfig(config.BucketName, config.AccessKeyId,
                config.AccessKeySecret, config.Endpoint, config.OssAccessUrlHttp,
                config.OssAccessUrlHttps, config.RootFolder, config.OssAccessUrlHttpAliyun, config.OssAccessUrlHttpsAliyun);
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
