using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using ETMS.Entity.Config;
using ETMS.IEventProvider;
using ETMS.Manage.Entity.Config;
using ETMS.Manage.Web.Core;
using ETMS.Manage.Web.Extensions;
using ETMS.ServiceBus;
using ETMS.Utility;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ETMS.Manage.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var manageSettingsSection = Configuration.GetSection("ManageSettings");
            services.Configure<ManageSettings>(manageSettingsSection);
            services.AddHangfire(x => x.UseSqlServerStorage(manageSettingsSection.Get<ManageSettings>().EtmsHangfireJobConnectionString));
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddSession();
            services.AddResponseCompression();
            services.AddResponseCaching();
            services.AddRedis(appSettingsSection.Get<AppSettings>().RedisConfig);
            services.AddControllersWithViews();
            InitCustomIoc(services);
        }

        private void InitCustomIoc(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpClient, StandardHttpClient>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            InitRabbitMq(builder);
            var jobAssembly = Assembly.Load("ETMS.Manage.Jobs");
            builder.RegisterAssemblyTypes(jobAssembly);
        }

        private void InitRabbitMq(ContainerBuilder container)
        {
            var config = Configuration.GetSection("AppSettings").Get<AppSettings>().RabbitMqConfig;
            var busControl = new SubscriptionAdapt().PublishAt(config.Host, "EtmsConsumerQueue", config.UserName, config.Password, config.Vhost, config.PrefetchCount);
            var publisher = new EventPublisher(busControl);
            container.RegisterInstance(publisher).As<IEventPublisher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseSession();
            app.UseResponseCompression();
            app.UseResponseCaching();

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangFireCustomAuthorizeFilter() }
            });
            JobProvider.InitJobs();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
