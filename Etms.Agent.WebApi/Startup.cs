using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Etms.Agent.WebApi.Extensions;
using Etms.Agent.WebApi.FilterAttribute;
using Etms.Agent.WebApi.Lib.Json;
using ETMS.Entity.Config;
using ETMS.IEventProvider;
using ETMS.IOC;
using ETMS.ServiceBus;
using ETMS.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Etms.Agent.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private const string PolicyName = "EtmsAgentDomainLimit";

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
                jsonOptions.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
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
            InitRabbitMq(builder);
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

        private void InitRabbitMq(ContainerBuilder container)
        {
            //var config = Configuration.GetSection("AppSettings").Get<AppSettings>().RabbitMqConfig;
            //var busControl = new SubscriptionAdapt().PublishAt(config.Host, "EtmsConsumerQueue", config.UserName, config.Password, config.Vhost, config.PrefetchCount);
            //var publisher = new EventPublisher(busControl);
            //container.RegisterInstance(publisher).As<IEventPublisher>();
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
