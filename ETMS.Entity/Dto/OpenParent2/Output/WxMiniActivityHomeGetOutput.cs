using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Output
{
    public class WxMiniActivityHomeGetOutput
    {
        /// <summary>
        /// 当前是否有活动主线
        /// </summary>
        public bool IsActivitying { get; set; }

        /// <summary>
        /// 是否是我的砍价
        /// <see cref="HaggleLogStatusOutput"/>
        /// </summary>
        public int HaggleLogStatus { get; set; }

        public bool IsCanHaggleLog { get; set; }

        public string ShareQRCode { get; set; }

        public WxMiniActivityHomeBascInfo BascInfo { get; set; }

        public WxMiniActivityHomeMyRoute TeamLeaderRoute { get; set; }

        public List<WxMiniActivityHomeJoinRoute> JoinRouteItems { get; set; }

        public List<WxMiniActivityHomeHaggleLog> HaggleLogs { get; set; }

        public WxMiniTenantConfig WxMiniTenantConfig { get; set; }
    }

    public class WxMiniTenantConfig
    {
        public string MicroWebHomeUrl { get; set; }
    }

    public struct HaggleLogStatusOutput
    {
        public const int OtherPeopleAndMyNone = 0;

        public const int OtherPeopleAndMyHave = 1;

        public const int My = 2;
    }

    public class WxMiniActivityHomeBascInfo
    {
        public long TenantId { get; set; }

        public long ActivityMainId { get; set; }

        public long? ActivityRouteId { get; set; }

        public long? ActivityRouteItemId { get; set; }

        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }

        public string ActivityTypeDesc { get; set; }

        public string ActivityTypeStyleClass { get; set; }

        public string ScenetypeStyleClass { get; set; }

        /// <summary>
        /// <see cref="EmActivityScenetype"/>
        /// </summary>
        public int Scenetype { get; set; }

        public string Name { get; set; }

        public string CoverImage { get; set; }

        public List<string> ImageMain { get; set; }

        public string TenantName { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string CourseName { get; set; }

        public string CourseDesc { get; set; }

        public List<string> ImageCourse { get; set; }

        public string OriginalPrice { get; set; }

        public string PayPriceDesc { get; set; }

        public string PayMustValue { get; set; }

        public WxMiniActivityHomeGroupPurchaseRule GroupPurchaseRule { get; set; }

        public string RuleEx1 { get; set; }

        public string RuleEx2 { get; set; }

        public string RuleEx3 { get; set; }

        public bool IsOpenPay { get; set; }

        /// <summary>
        /// <see cref="EmActivityPayType"/>
        /// </summary>
        public int PayType { get; set; }

        public decimal PayValue { get; set; }

        public int MaxCount { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EmBool"/>
        /// </summary>
        public byte StudentHisLimitType { get; set; }

        public string ActivityExplan { get; set; }

        public string TenantLinkInfo { get; set; }

        public string TenantLinkQRcode { get; set; }

        public string TenantIntroduceTxt { get; set; }

        public List<string> TenantIntroduceImg { get; set; }

        public string GlobalPhone { get; set; }

        public bool GlobalOpenBullet { get; set; }

        public bool GlobalOpenStatistics { get; set; }

        public bool IsOpenCheckPhone { get; set; }

        public int PVCount { get; set; }

        public int UVCount { get; set; }

        public int TranspondCount { get; set; }

        public int VisitCount { get; set; }

        public int JoinCount { get; set; }

        public int RouteCount { get; set; }

        public int RuningCount { get; set; }

        public int FinishCount { get; set; }

        public int FinishFullCount { get; set; }

        public int FailCount { get; set; }

        public string ShareQRCode { get; set; }

        /// <summary>
        /// <see cref="EmActivityStyleType"/>
        /// </summary>
        public int StyleType { get; set; }

        public string StyleBackColor { get; set; }

        public string StyleColumnColor { get; set; }

        public string StyleColumnImg { get; set; }

        public string StudentFieldName1 { get; set; }

        public string StudentFieldName2 { get; set; }

        public DateTime? PublishTime { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// <see cref="EmActivityStatus"/>
        /// </summary>
        public int ActivityStatus { get; set; }

        public string ActivityStatusDesc { get; set; }

        public bool IsMultiGroupPurchase { get; set; }
    }

    public class WxMiniActivityHomeGroupPurchaseRule
    {
        public int TotalLimitCount { get; set; }
        public List<WxMiniActivityHomeGroupPurchaseRuleItem> Items { get; set; }
    }

    public class WxMiniActivityHomeGroupPurchaseRuleItem
    {
        public int LimitCount { get; set; }

        public decimal Money { get; set; }

        public decimal Length { get; set; }
    }

    public class WxMiniActivityHomeMyRoute
    {
        public long ActivityRouteId { get; set; }

        public long MiniPgmUserId { get; set; }

        public string AvatarUrl { get; set; }

        public string StudentNameDesc { get; set; }

        public int CountLimit { get; set; }

        public int CountFinish { get; set; }

        public int CountShort { get; set; }

        public byte CountShortStatus { get; set; }

        public List<WxMiniActivityHomeJoinRouteItemSmall> JoinRouteItems { get; set; }
    }

    public class WxMiniActivityHomeJoinRoute
    {
        public long ActivityRouteId { get; set; }

        public long MiniPgmUserId { get; set; }

        public string AvatarUrl { get; set; }

        public string StudentNameDesc { get; set; }

        public int CountLimit { get; set; }

        public int CountFinish { get; set; }

        public int CountShort { get; set; }

        public byte CountShortStatus { get; set; }

        public string CurrentAvatarUrl { get; set; }

        public string CurrentStudentNameDesc { get; set; }

        public int CurrentIndex { get; set; }

        public List<WxMiniActivityHomeJoinRouteItem> JoinRouteItems { get; set; }
    }

    public class WxMiniActivityHomeJoinRouteItem
    {
        public long ActivityRouteId { get; set; }

        public long ActivityRouteItemId { get; set; }

        public long MiniPgmUserId { get; set; }

        public string StudentNameDesc { get; set; }

        public string AvatarUrl { get; set; }

        public bool IsTeamLeader { get; set; }
    }

    public class WxMiniActivityHomeJoinRouteItemSmall {
        public long ActivityRouteId { get; set; }

        public long ActivityRouteItemId { get; set; }

        public long MiniPgmUserId { get; set; }

        public string AvatarUrl { get; set; }

        public bool IsTeamLeader { get; set; }
    }

    public class WxMiniActivityHomeHaggleLog
    {
        public long MiniPgmUserId { get; set; }

        public string NickName { get; set; }

        public string AvatarUrl { get; set; }
    }
}
