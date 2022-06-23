using ETMS.Entity.Dto.Activity.Request;
using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Output
{
    public class ActivityMainGetForEditOutput
    {
        public long ActivityMainId { get; set; }

        public long SystemId { get; set; }

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

        public List<GroupPurchaseRuleInput> GroupPurchaseRuleInputs { get; set; }

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

        public string ActivityTypeStyleClass { get; set; }

        public string ScenetypeStyleClass { get; set; }

        public bool GlobalOpenStatistics { get; set; }

        public bool IsOpenStudentFieldName1 { get; set; }

        public bool IsOpenStudentFieldName2 { get; set; }
    }
}
