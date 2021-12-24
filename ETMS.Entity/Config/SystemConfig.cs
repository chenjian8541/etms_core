using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Config
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SystemConfig
    {
        /// <summary>
        /// Jwt授权配置
        /// </summary>
        public static AuthenticationConfig AuthenticationConfig;

        /// <summary>
        /// 用户登录配置
        /// </summary>
        public static UserLoginConfig UserLoginConfig;

        /// <summary>
        /// 加密配置
        /// </summary>
        public static CryptogramConfig CryptogramConfig;

        /// <summary>
        /// 用户安全
        /// </summary>
        public static UserSafetyConfig UserSafetyConfig;

        public static ParentAccessConfig ParentAccessConfig;

        public static ClssTimesConfig ClssTimesConfig;

        public static SysSafetyConfig SysSafetyConfig;

        public static TenantDefaultConfig TenantDefaultConfig;

        public static ComConfig ComConfig;

        /// <summary>
        /// 静态构造函数
        /// 初始化内部静态成员
        /// </summary>
        static SystemConfig()
        {
            AuthenticationConfig = new AuthenticationConfig();
            UserLoginConfig = new UserLoginConfig();
            CryptogramConfig = new CryptogramConfig();
            UserSafetyConfig = new UserSafetyConfig();
            ParentAccessConfig = new ParentAccessConfig();
            ClssTimesConfig = new ClssTimesConfig();
            SysSafetyConfig = new SysSafetyConfig();
            TenantDefaultConfig = new TenantDefaultConfig();
            ComConfig = new ComConfig();
        }
    }

    public class TenantDefaultConfig
    {
        /// <summary>
        /// 机构试用30天
        /// </summary>
        public int TenantTestDay = 30;

        /// <summary>
        /// 默认限制机构用户数量100个
        /// </summary>
        public int MaxUserCountDefault = 100;
    }

    /// <summary>
    /// 学员端访问配置
    /// </summary>
    public class ParentAccessConfig
    {
        /// <summary>
        /// 访问签名key
        /// </summary>
        public string SignatureSecret = "716041c02ed48c8b429972e5c31ec341";

        /// <summary>
        /// 登陆验证码失效时长(单位分钟)
        /// </summary>
        public int LoginSmsCodeTimeOut = 10;
    }

    /// <summary>
    /// 用户安全
    /// </summary>
    public class UserSafetyConfig
    {
        /// <summary>
        /// 用户修改密码验证码失效时长(单位分钟)  
        /// </summary>
        public int UserChangePwdSmsCodeTimeOut = 10;
    }

    /// <summary>
    /// 加密配置
    /// </summary>
    public class CryptogramConfig
    {
        /// <summary>
        /// 加解密密钥
        /// </summary>
        public string Key = "etms88888888";
    }

    /// <summary>
    /// JWT授权配置
    /// </summary>
    public class AuthenticationConfig
    {
        /// <summary>
        /// jwt用户类型名称
        /// </summary>
        public string ClaimType = "etms_user";

        /// <summary>
        /// 默认的身份验证方案名称
        /// </summary>
        public string DefaultAuthenticateScheme = "JwtBearer";

        /// <summary>
        /// 默认方案名称
        /// </summary>
        public string DefaultChallengeScheme = "JwtBearer";

        /// <summary>
        /// 令牌发行者名称
        /// </summary>
        public string ValidIssuer = "www.61vip.cn";

        /// <summary>
        /// 获取令牌用户名称
        /// </summary>
        public string ValidAudience = "etms_user";

        /// <summary>
        /// 签名密钥
        /// </summary>
        public string IssuerSigningKey = "ETMS.JWT.2d88d1def5494c81ac5ce548fcd7a4ae";

        /// <summary>
        /// 是否验证过期时间
        /// </summary>
        public bool ValidateLifetime = true;

        /// <summary>
        /// 默认过期时间(30天过期)
        /// </summary>
        public int ExpiresDay = 30;

        /// <summary>
        /// 验证过期时间时应用的时钟偏差(单位分钟)
        /// </summary>
        public int ClockSkew = 2;
    }

    /// <summary>
    /// 用户登录配置
    /// </summary>
    public class UserLoginConfig
    {
        /// <summary>
        /// 每个时间段内登录失败的最大数
        /// </summary>
        public int LoginFailedMaxCount = 5;

        /// <summary>
        /// 登录失败时禁止登录时长(单位分钟)
        /// </summary>
        public int LoginFailedTimeOut = 10;

        /// <summary>
        /// 登陆验证码失效时长(单位分钟)
        /// </summary>
        public int LoginSmsCodeTimeOut = 10;
    }

    public class ClssTimesConfig
    {
        /// <summary>
        /// 一个班级最多可以创建的排课规则数
        /// </summary>
        public int ClassTimesRuleMaxCount = 100;

        /// <summary>
        /// 创建班级排课规则时预生成的课次数量
        /// </summary>
        public int PreGenerateClassTimesCount = 100;
    }

    public class SysSafetyConfig
    {
        /// <summary>
        /// 安全验证短信过期时间(单位分钟)
        /// </summary>
        public int SysSafeSmsCodeTimeOut = 10;
    }

    public class ComConfig
    {
        /// <summary>
        /// 一个月的天数
        /// </summary>
        public int MonthToDay = 30;

        /// <summary>
        /// 课程默认颜色
        /// </summary>
        public string CourseDefaultStyleColor = "#78BFFA";

        /// <summary>
        /// 老师工资 绩效工资默认Id
        /// </summary>
        public long TeacherSalaryPerformanceDefaultId = -10;

        /// <summary>
        /// 扫呗支付(限支付30天内退款，超过30天，不能进行退款操作)
        /// </summary>
        public int LcsRefundOrderLimitDay = 30;

        /// <summary>
        /// 老师端默认显示菜单值
        /// </summary>
        public string UserH5HomeMenusDefault = "2046";

        /// <summary>
        /// 系统即将到期提醒
        /// </summary>
        public int SystemExpireDayLimit = 30;
    }
}
