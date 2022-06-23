using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Output
{
    public class ActivityMainCreateInitOfHaggleOutput
    {
        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }

        /// <summary>
        /// <see cref="EmActivityScenetype"/>
        /// </summary>
        public int Scenetype { get; set; }

        public string SystemId { get; set; }

        public string Name { get; set; }

        public List<string> ImageMains { get; set; }

        public string TenantName { get; set; }

        public string Title { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string CourseName { get; set; }

        public string CourseDesc { get; set; }

        public List<string> ImageCourse { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal LowPrice { get; set; }

        public int LimitMustCount { get; set; }

        public int MyRepeatHaggleHour { get; set; }

        public bool IsAllowPay { get; set; }

        public bool IsOpenPay { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.EtmsManage.EmActivityPayType"/>
        /// </summary>
        public int PayType { get; set; }

        public decimal PayValue { get; set; }

        public int MaxCount { get; set; }

        public bool IsLimitStudent { get; set; }

        public string ActivityExplan { get; set; }

        public string TenantLinkInfo { get; set; }

        public string TenantLinkQRcode { get; set; }

        public string TenantIntroduceTxt { get; set; }

        public List<string> TenantIntroduceImg { get; set; }

        public string GlobalPhone { get; set; }

        public bool GlobalOpenBullet { get; set; }

        public bool IsOpenCheckPhone { get; set; }

        public string StudentFieldName1 { get; set; }

        public string StudentFieldName2 { get; set; }

        public bool IsOpenStudentFieldName1 { get; set; }

        public bool IsOpenStudentFieldName2 { get; set; }

        public bool GlobalOpenStatistics { get; set; }

        /// <summary>
        /// <see cref="EmActivityStyleType"/>
        /// </summary>
        public int StyleType { get; set; }

        public string StyleBackColor { get; set; }

        public string StyleColumnColor { get; set; }

        public string StyleColumnImg { get; set; }
    }
}
