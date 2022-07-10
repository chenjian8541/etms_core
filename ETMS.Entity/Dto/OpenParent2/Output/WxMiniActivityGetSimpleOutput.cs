using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.OpenParent2.Output
{
    public class WxMiniActivityGetSimpleOutput
    {
        public long TenantId { get; set; }

        public long ActivityMainId { get; set; }

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

        public string TenantName { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string CourseName { get; set; }

        public decimal OriginalPrice { get; set; }

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


        public string GlobalPhone { get; set; }

        public bool GlobalOpenBullet { get; set; }

        public bool GlobalOpenStatistics { get; set; }

        public bool IsOpenCheckPhone { get; set; }
      

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

        public WxMiniActivityHomeMyRoute TeamLeaderRoute { get; set; }
    }
}
