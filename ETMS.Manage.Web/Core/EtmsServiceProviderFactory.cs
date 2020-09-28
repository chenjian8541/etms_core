using Autofac;
using Autofac.Extensions.DependencyInjection;
using ETMS.IOC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETMS.Manage.Web.Core
{
    public class EtmsServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private AutofacServiceProviderFactory _autofacServiceProviderFactory;

        public EtmsServiceProviderFactory(Action<ContainerBuilder> configurationAction = null)
        {
            _autofacServiceProviderFactory = new AutofacServiceProviderFactory(configurationAction);
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = _autofacServiceProviderFactory.CreateBuilder(services);
            Bootstrapper.Initialize(builder);
            return builder;
        }
        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            var service = _autofacServiceProviderFactory.CreateServiceProvider(containerBuilder);
            Bootstrapper.InitCustomServiceLocator(service);
            return service;
        }
    }
}
