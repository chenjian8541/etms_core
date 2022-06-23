using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Request
{
    public class ActivityMainSaveOrPublishOfHaggleRequest : RequestBase
    {
        public string SystemId { get; set; }

        public string Name { get; set; }

        public List<string> ImageMains { get; set; }

        public string TenantName { get; set; }

        public string Title { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string CourseName { get; set; }

        public string CourseDesc { get; set; }

        public List<string> ImageCourse { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal LowPrice { get; set; }

        public int LimitMustCount { get; set; }

        public int MyRepeatHaggleHour { get; set; }

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

        public bool GlobalOpenStatistics { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(SystemId))
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入活动名称";
            }
            if (ImageMains == null || ImageMains.Count == 0)
            {
                return "请上传活动主图";
            }
            if (string.IsNullOrEmpty(TenantName))
            {
                return "请输入机构名称";
            }
            if (string.IsNullOrEmpty(Title))
            {
                return "请输入活动标题";
            }
            if (StartTime == null)
            {
                return "请选择活动开始时间";
            }
            if (EndTime == null)
            {
                return "请选择活动结束时间";
            }
            if (EndTime <= DateTime.Now)
            {
                return "结束时间必须大于当前时间";
            }
            if (StartTime >= EndTime)
            {
                return "结束时间必须大于开始时间";
            }
            if (string.IsNullOrEmpty(CourseName))
            {
                return "请输入课程名称";
            }
            if (OriginalPrice <= 0)
            {
                return "请输入原价";
            }
            if (LowPrice < 0)
            {
                return "请输入底价";
            }
            if (LowPrice >= OriginalPrice)
            {
                return "底价必须低于原价";
            }
            if (LimitMustCount <= 1 || LimitMustCount > 200)
            {
                return "活动难度在2~200之间，请重新输入";
            }
            if (MaxCount <= 0)
            {
                return "课程数量必须大于0";
            }
            return string.Empty;
        }
    }
}
