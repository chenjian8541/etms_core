using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ETMS.Entity.Enum.EtmsManage;

namespace ETMS.Entity.Database.Manage
{
    [Table("SysActivity")]
    public class SysActivity : EManageEntity<long>
    {
        /// <summary>
        /// <see cref="EmActivityType"/>
        /// </summary>
        public int ActivityType { get; set; }

        public string ActivityTypeStyleClass { get; set; }

        public string ScenetypeStyleClass { get; set; }

        public string ActivityTypeDesc { get; set; }

        /// <summary>
        /// <see cref="EmActivityScenetype"/>
        /// </summary>
        public int Scenetype { get; set; }

        public string ScenetypeDesc { get; set; }

        public string Name { get; set; }

        public string CoverImage { get; set; }

        public string ImageMain { get; set; }

        public string Title { get; set; }

        public string CourseName { get; set; }

        public string CourseDesc { get; set; }

        public string ImageCourse { get; set; }

        public decimal OriginalPrice { get; set; }

        public string RuleContent { get; set; }

        public string RuleEx1 { get; set; }

        public string RuleEx2 { get; set; }

        public string RuleEx3 { get; set; }

        public bool IsAllowPay { get; set; }

        public int MaxCount { get; set; }

        public string ActivityExplan { get; set; }

        public string TenantLinkInfo { get; set; }

        /// <summary>
        /// <see cref="EmActivityStyleType"/>
        /// </summary>
        public int StyleType { get; set; }

        public string StyleBackColor { get; set; }

        public string StyleColumnColor { get; set; }
    }
}
