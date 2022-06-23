using ETMS.Entity.Common;
using ETMS.Entity.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Activity.Request
{
    public class ActivityMainSaveOfGroupPurchaseRequest : RequestBase
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

        public List<GroupPurchaseRuleInput> GroupPurchaseRuleInputs { get; set; }

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
            if (GroupPurchaseRuleInputs == null || GroupPurchaseRuleInputs.Count == 0)
            {
                return "请填写拼团方案";
            }
            string errMsg;
            foreach (var item in GroupPurchaseRuleInputs)
            {
                errMsg = item.Validate();
                if (!string.IsNullOrEmpty(errMsg))
                {
                    return errMsg;
                }
            }
            if (MaxCount <= 0)
            {
                return "支付人数上限必须大于0";
            }
            var maxLimitCount = GroupPurchaseRuleInputs.OrderByDescending(j => j.LimitCount).First().LimitCount;
            if (MaxCount < maxLimitCount)
            {
                return "支付人数上线必须大于拼团人数";
            }
            if (GroupPurchaseRuleInputs.Count > 1)
            {
                if (IsOpenPay && PayType == Enum.EtmsManage.EmActivityPayType.DiscountSum)
                {
                    return "多阶拼团模式下，则仅支持“定金”支付";
                }
            }
            if (GroupPurchaseRuleInputs.Count > 5)
            {
                return "最多设置5组拼团方案";
            }
            if (IsOpenPay)
            {
                if (PayType == Enum.EtmsManage.EmActivityPayType.Deposit && PayValue <= 0)
                {
                    return "请输入支付金额";
                }
            }
            return string.Empty;
        }
    }

    public class GroupPurchaseRuleInput : IValidate
    {
        public int LimitCount { get; set; }

        public decimal Money { get; set; }

        public string Validate()
        {
            if (LimitCount <= 1)
            {
                return "拼团方案人数必须大于1";
            }
            if (LimitCount > 200)
            {
                return "拼团方案人数必须小于200人";
            }
            if (Money <= 0)
            {
                return "拼团价格必须大于0";
            }
            return string.Empty;
        }
    }
}
