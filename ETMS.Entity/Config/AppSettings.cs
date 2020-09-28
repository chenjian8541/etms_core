using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 数据库配置
        /// </summary>
        public DatabseConfig DatabseConfig { get; set; }

        /// <summary>
        /// Redis配置
        /// </summary>
        public RedisConfig RedisConfig { get; set; }

        /// <summary>
        /// RabbitMq 配置
        /// </summary>
        public RabbitMqConfig RabbitMqConfig { get; set; }

        /// <summary>
        /// 短信配置
        /// </summary>
        public SmsConfig SmsConfig { get; set; }

        public StaticFilesConfig StaticFilesConfig { get; set; }

        public ServerConfig ServerConfig { get; set; }

        /// <summary>
        /// 家长端配置
        /// </summary>
        public ParentConfig ParentConfig { get; set; }
    }

    public class ServerConfig
    {
        /// <summary>
        ///  系统类型  <see cref="ETMS.Entity.Enum.EmServerConfigType"/>
        /// </summary>
        public byte Type { get; set; }
    }

    public class StaticFilesConfig
    {
        public string ServerPath { get; set; }

        public string VirtualPath { get; set; }

        public long UploadFileSizeLimit { get; set; }

        public string UploadImageFileTypeLimit { get; set; }
    }

    /// <summary>
    /// 短信配置
    /// </summary>
    public class SmsConfig
    {
        /// <summary>
        /// 凌凯提供商
        /// </summary>
        public ZhuTong ZhuTong { get; set; }

    }

    /// <summary>
    /// 助通供应商  
    /// 接口地址：https://doc.zthysms.com/web/#/1?page_id=13
    /// </summary>
    public class ZhuTong
    {
        public string SendSmsTpUrl { get; set; }

        public string SendSms { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Signature { get; set; }

        /// <summary>
        /// 员工登录获取验证码
        /// </summary>
        public TemplatesLogin TemplatesLogin { get; set; }

        /// <summary>
        /// 家长登录获取验证码
        /// </summary>
        public TemplatesParentLogin TemplatesParentLogin { get; set; }

        public NoticeStudentsOfClassBeforeDay NoticeStudentsOfClassBeforeDay { get; set; }

        public NoticeStudentsOfClassToday NoticeStudentsOfClassToday { get; set; }

        public ClassCheckSign ClassCheckSign { get; set; }

        public StudentLeaveApply StudentLeaveApply { get; set; }

        public StudentContracts StudentContracts { get; set; }
    }

    public class StudentContracts
    {
        public string Com { get; set; }
    }

    public class StudentLeaveApply
    {
        public string Com { get; set; }
    }

    public class ClassCheckSign
    {
        public string Com { get; set; }
    }

    public class NoticeStudentsOfClassBeforeDay
    {
        public string HasRoom { get; set; }

        public string NoRoom { get; set; }
    }

    public class NoticeStudentsOfClassToday
    {
        public string HasRoom { get; set; }

        public string NoRoom { get; set; }
    }

    public class TemplatesLogin
    {
        public long TpId { get; set; }
    }

    public class TemplatesParentLogin
    {
        public long TpId { get; set; }
    }

    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DatabseConfig
    {
        /// <summary>
        /// EtmsManage库连接字符串
        /// </summary>
        public string EtmsManageConnectionString { get; set; }
    }

    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// 可写服务器
        /// </summary>
        public string ServerConStrFormat { get; set; }

        /// <summary>
        /// 可使用的数据库数量(如果此值位5，则表示0~4db)
        /// </summary>
        public int DbCount { get; set; }
    }

    /// <summary>
    /// RabbitMq配置
    /// </summary>
    public class RabbitMqConfig
    {
        /// <summary>
        /// RabbitMq服务器
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// vhost
        /// </summary>
        public string Vhost { get; set; }

        /// <summary>
        /// PrefetchCount
        /// </summary>
        public ushort PrefetchCount { get; set; }
    }

    /// <summary>
    /// 家长端配置
    /// </summary>
    public class ParentConfig
    {

        /// <summary>
        /// token 过期天数
        /// </summary>
        public int TokenExpiredDay { get; set; }
    }
}
