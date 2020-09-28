using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace ETMS.IOC
{
    /// <summary>
    /// 依赖注入程序入口
    /// </summary>
    public class Bootstrapper
    {
        /// <summary>
        /// 依赖注入程序入口
        /// </summary>
        /// <param name="contractProcess"></param>
        /// <returns></returns>
        public static IServiceProvider Bootstrap(Action<ContainerBuilder> contractProcess = null)
        {
            var builder = new ContainerBuilder();
            builder.Initialize();
            contractProcess?.Invoke(builder);
            var container = builder.Build();
            var provider = new AutofacServiceProvider(container);
            CustomServiceLocator.InitCustomServiceLocator(provider);
            return provider;
        }

        /// <summary>
        /// 初始化注册项
        /// </summary>
        /// <param name="builder"></param>
        public static void Initialize(ContainerBuilder builder)
        {
            builder.Initialize();
        }

        /// <summary>
        /// 初始化服务定位器
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void InitCustomServiceLocator(IServiceProvider serviceProvider)
        {
            CustomServiceLocator.InitCustomServiceLocator(serviceProvider);
        }
    }
}
