using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ETMS.Utility;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentEnrolmentRequest : RequestBase
    {
        public long StudentId { get; set; }

        public List<EnrolmentCourse> EnrolmentCourses { get; set; }

        public List<EnrolmentGoods> EnrolmentGoodss { get; set; }

        public List<EnrolmentCost> EnrolmentCosts { get; set; }

        public List<long> CouponsStudentGetIds { get; set; }

        public EnrolmentPayInfo EnrolmentPayInfo { get; set; }

        public OtherInfo OtherInfo { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0)
            {
                return "请求数据格式错误";
            }
            if ((EnrolmentCourses == null || !EnrolmentCourses.Any())
                && (EnrolmentGoodss == null || !EnrolmentGoodss.Any())
                && (EnrolmentCosts == null || !EnrolmentCosts.Any()))
            {
                return "请选择需要购买的课程、物品或费用";
            }
            if (EnrolmentCourses != null && EnrolmentCourses.Any())
            {
                foreach (var p in EnrolmentCourses)
                {
                    var msg = p.Validate();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return msg;
                    }
                }
            }
            if (EnrolmentGoodss != null && EnrolmentGoodss.Any())
            {
                foreach (var p in EnrolmentGoodss)
                {
                    var msg = p.Validate();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return msg;
                    }
                }
            }
            if (EnrolmentCosts != null && EnrolmentCosts.Any())
            {
                foreach (var p in EnrolmentCosts)
                {
                    var msg = p.Validate();
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return msg;
                    }
                }
            }
            if (EnrolmentPayInfo == null)
            {
                return "请填写支付信息";
            }
            var msgPay = EnrolmentPayInfo.Validate();
            if (!string.IsNullOrEmpty(msgPay))
            {
                return msgPay;
            }
            if (OtherInfo == null)
            {
                return "请填写其他信息";
            }
            var msgOther = OtherInfo.Validate();
            if (!string.IsNullOrEmpty(msgOther))
            {
                return msgOther;
            }
            return base.Validate();
        }
    }


    public class OtherInfo : IValidate
    {
        public DateTime Ot { get; set; }

        public List<long> CommissionUser { get; set; }

        public string Remark { get; set; }

        public int TotalPoints { get; set; }

        public string Validate()
        {
            if (!Ot.IsEffectiveDate())
            {
                return "请选择有效的经办日期";
            }
            return string.Empty;
        }
    }
    public class EnrolmentPayInfo : IValidate
    {
        public decimal PayWechat { get; set; }

        public decimal PayAlipay { get; set; }

        public decimal PayCash { get; set; }

        public decimal PayBank { get; set; }

        public decimal PayPos { get; set; }

        public decimal PayAccountRechargeReal { get; set; }

        public decimal PayAccountRechargeGive { get; set; }

        public long? PayAccountRechargeId { get; set; }

        public string Validate()
        {
            return string.Empty;
        }
    }

    public class EnrolmentCost : IValidate
    {
        public long CostId { get; set; }

        public int BuyQuantity { get; set; }

        /// <summary>
        /// 折扣  <see cref="ETMS.Entity.Enum.EmDiscountType"/>
        /// </summary>
        public byte DiscountType { get; set; }

        /// <summary>
        /// 优惠值
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// 应收金额（优惠完的金额）
        /// </summary>
        public decimal ItemAptSum { get; set; }

        public string Validate()
        {
            if (CostId <= 0)
            {
                return "请求数据格式错误";
            }
            if (BuyQuantity <= 0)
            {
                return "购买数量必须大于0";
            }
            return string.Empty;
        }
    }

    public class EnrolmentGoods : IValidate
    {
        public long GoodsId { get; set; }

        public int BuyQuantity { get; set; }

        /// <summary>
        /// 折扣  <see cref="ETMS.Entity.Enum.EmDiscountType"/>
        /// </summary>
        public byte DiscountType { get; set; }

        /// <summary>
        /// 优惠值
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// 应收金额（优惠完的金额）
        /// </summary>
        public decimal ItemAptSum { get; set; }

        public string Validate()
        {
            if (GoodsId <= 0)
            {
                return "请求数据格式错误";
            }
            if (BuyQuantity <= 0)
            {
                return "购买数量必须大于0";
            }
            return string.Empty;
        }
    }

    public class EnrolmentCourse : IValidate
    {
        public long CourseId { get; set; }

        public long CoursePriceRuleId { get; set; }

        /// <summary>
        /// 收费标准中 如果数量大于1  则取收费标准中的数量
        /// </summary>
        public int BuyQuantity { get; set; }

        public int GiveQuantity { get; set; }

        /// <summary>
        ///单位 <see cref=" ETMS.Entity.Enum.EmCourseUnit"/>
        /// </summary>
        public byte GiveUnit { get; set; }

        /// <summary>
        /// 折扣  <see cref="ETMS.Entity.Enum.EmDiscountType"/>
        /// </summary>
        public byte DiscountType { get; set; }

        /// <summary>
        /// 优惠值
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// 应收金额（优惠完的金额）
        /// </summary>
        public decimal ItemAptSum { get; set; }

        /// <summary>
        /// 起止日期
        /// </summary>
        public List<string> ErangeOt { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public string ExOt { get; set; }

        public string Validate()
        {
            if (CourseId <= 0 || CoursePriceRuleId <= 0)
            {
                return "请求数据格式错误";
            }
            if (BuyQuantity <= 0)
            {
                return "购买数量必须大于0";
            }
            return string.Empty;
        }
    }
}
