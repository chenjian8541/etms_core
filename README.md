使用MassTransit来集成rabbitmq服务
https://github.com/chenjian8541/etms_core/blob/master/ETMS.ServiceBus/SubscriptionAdapt2.cs

rabbitmq消费者订阅消息
https://github.com/chenjian8541/etms_core/blob/master/ETMS.DaemonService/ServiceProvider.cs

缓存与db同步
https://github.com/chenjian8541/etms_core/blob/master/ETMS.DataAccess/Lib/BaseCacheDAL%5BT%5D.cs

权限校验
https://github.com/chenjian8541/etms_core/blob/master/ETMS.Authority/AuthorityCore2.cs

使用Autofac IOC容器，并实现一个服务定位器
https://github.com/chenjian8541/etms_core/blob/master/ETMS.WebApi/Core/EtmsServiceProviderFactory.cs
https://github.com/chenjian8541/etms_core/blob/master/ETMS.IOC/Bootstrapper.cs
https://github.com/chenjian8541/etms_core/blob/master/ETMS.IOC/ContainerBuilderExtend.cs
https://github.com/chenjian8541/etms_core/blob/master/ETMS.IOC/CustomServiceLocator.cs

hangfire定时job，使用json配置job规则，使用反射找到job处理程序
https://github.com/chenjian8541/etms_core/blob/master/ETMS.Manage.Web/Core/JobProvider.cs

请求参数校验逻辑，并对请求上下文对象赋值(模仿abp)
https://github.com/chenjian8541/etms_core/blob/master/ETMS.WebApi/FilterAttribute/EtmsValidateRequestAttribute.cs

JWT授权
https://github.com/chenjian8541/etms_core/blob/master/ETMS.WebApi/Extensions/ServiceCollectionExtensions.cs

自定义签名授权验证
https://github.com/chenjian8541/etms_core/blob/master/ETMS.WebApi/FilterAttribute/EtmsSignatureWxminiAuthorizeAttribute.cs
https://github.com/chenjian8541/etms_core/blob/master/ETMS.WebApi/FilterAttribute/EtmsSignatureParentAuthorizeAttribute.cs

多租户下对应的redis不同的库
https://github.com/chenjian8541/etms_core/blob/master/ETMS.Cache.Redis/Wrapper/CSRedisWrapper.cs

解决.net core webapi返回长整形数据到前端时精度丢失的bug（js设计缺陷导致的）
https://github.com/chenjian8541/etms_core/blob/master/ETMS.WebApi/Lib/Json/EtmsContractResolver.cs
https://github.com/chenjian8541/etms_core/blob/master/ETMS.WebApi/Lib/Json/JsonConverterLong.cs

分布式事务锁执行处理程序
https://github.com/chenjian8541/etms_core/blob/master/ETMS.EventConsumer/Lib/LockTakeHandler.cs
https://github.com/chenjian8541/etms_core/blob/master/ETMS.EventConsumer/AutoCheckSignTenantConsumer.cs




