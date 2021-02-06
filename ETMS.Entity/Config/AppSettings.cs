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

        public WxConfig WxConfig { get; set; }

        public UserConfig UserConfig { get; set; }

        public SenparcConfig SenparcConfig { get; set; }

        public AliyunOssConfig AliyunOssConfig { get; set; }

        public MailConfig MailConfig { get; set; }
    }

    public class MailConfig
    {
        public string SenderAddress { get; set; }

        public string SenderDisplayName { get; set; }

        public string SenderUserName { get; set; }

        public string SenderPassword { get; set; }

        public string MailHost { get; set; }

        public int MailPort { get; set; }

        public string[] SystemDataBackupsGetUser { get; set; }

        public string SystemDataBackupsSearchPattern { get; set; }

        public string SystemDataBackupsServerPath { get; set; }
    }

    public class AliyunOssConfig
    {
        public string BucketName { get; set; }

        public string AccessKeyId { get; set; }

        public string AccessKeySecret { get; set; }

        public string Endpoint { get; set; }

        public string OssAccessUrlHttp { get; set; }

        public string OssAccessUrlHttps { get; set; }

        public string RootFolder { get; set; }
    }

    public class SenparcConfig
    {
        public bool CheckPublish { get; set; }

        public SenparcSetting SenparcSetting { get; set; }

        public SenparcWeixinSetting SenparcWeixinSetting { get; set; }
    }

    public class SenparcSetting
    {
        public bool IsDebug { get; set; }

        public string DefaultCacheNamespace { get; set; }

        public string CacheRedisConfiguration { get; set; }

        public string SenparcUnionAgentKey { get; set; }
    }

    public class SenparcWeixinSetting
    {
        public ComponentConfig ComponentConfig { get; set; }
    }

    public class ComponentConfig
    {
        public string ComponentAppid { get; set; }

        public string ComponentSecret { get; set; }

        public string ComponentToken { get; set; }

        public string ComponentEncodingAESKey { get; set; }

        public string ComponentOAuthCallbackUrl { get; set; }
    }

    public class UserConfig
    {
        public List<string> LoginWhitelistTenantUser { get; set; }
    }

    public class WxConfig
    {
        public TemplateNoticeConfig TemplateNoticeConfig { get; set; }

        public WeChatEntranceConfig WeChatEntranceConfig { get; set; }
    }

    public class WeChatEntranceConfig
    {
        public string ParentLogin { get; set; }

        public string TeacherLogin { get; set; }
    }

    public class TemplateNoticeConfig
    {
        public string NoticeStudentsOfClass { get; set; }

        public string StudentContracts { get; set; }

        public string StudentLeaveApply { get; set; }

        public string ClassCheckSign { get; set; }

        public string HomeworkAdd { get; set; }

        public string HomeworkExpireRemind { get; set; }

        public string HomeworkComment { get; set; }

        public string GrowthRecordAdd { get; set; }

        public string WxMessage { get; set; }

        public string StudentEvaluate { get; set; }

        public string StudentCourseSurplus { get; set; }

        public string StudentMakeup { get; set; }

        public string StudentCourseNotEnough { get; set; }

        public string StudentCourseNotEnoughClassTimes { get; set; }

        public string StudentCourseNotEnoughClassDay { get; set; }

        public string NoticeUserOfClass { get; set; }

        public string NoticeUserOfHomeworkFinish { get; set; }

        public string StudentCheckIn { get; set; }

        public string StudentCheckOut { get; set; }

        public string ClassRecordDetailFrontUrl { get; set; }

        public string StudentLeaveApplyDetailFrontUrl { get; set; }

        public string StudentOrderDetailFrontUrl { get; set; }

        public string StudentHomeworkDetailUrl { get; set; }

        public string StudentGrowthRecordDetailUrl { get; set; }

        public string StudentWxMessageDetailUrl { get; set; }

        public string StudentCourseUrl { get; set; }

        public string StudentCheckLogUrl { get; set; }

        public string CouponsUrl { get; set; }
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

        public string UploadExcelFileTypeLimit { get; set; }

        public long UploadVideoFileSizeLimit { get; set; }

        public string UploadVideoFileTypeLimit { get; set; }

        public long UploadAudioFileSizeLimit { get; set; }

        public string UploadAudioFileTypeLimit { get; set; }
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

        public StudentCourseNotEnough StudentCourseNotEnough { get; set; }

        public NoticeUserOfClassToday NoticeUserOfClassToday { get; set; }

        public StudentCheckOnLog StudentCheckOnLog { get; set; }
    }

    public class StudentCheckOnLog
    {
        public string CheckIn { get; set; }

        public string CheckOut { get; set; }
    }

    public class NoticeUserOfClassToday
    {
        public string HasRoom { get; set; }

        public string NoRoom { get; set; }
    }

    public class StudentCourseNotEnough
    {
        public string Com { get; set; }
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

        /// <summary>
        /// hangfire jobs setting
        /// </summary>
        public string EtmsHangfireJobConnectionString { get; set; }
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
        /// redis
        /// </summary>
        public string ServerConStrDefault { get; set; }

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
