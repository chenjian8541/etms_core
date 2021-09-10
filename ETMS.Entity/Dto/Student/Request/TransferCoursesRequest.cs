using ETMS.Entity.Common;
using ETMS.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class TransferCoursesRequest : RequestBase
    {
        public long StudentId { get; set; }

        /// <summary>
        ///转出的课程
        /// </summary>
        public long CourseId { get; set; }

        public List<TransferCoursesOut> TransferCoursesOut { get; set; }

        public List<TransferCoursesBuy> TransferCoursesBuy { get; set; }

        public TransferCoursesOrderInfo TransferCoursesOrderInfo { get; set; }

        public override string Validate()
        {
            if (StudentId <= 0 || CourseId <= 0)
            {
                return "请求数据格式错误";
            }
            if (TransferCoursesOut == null || TransferCoursesOut.Count == 0)
            {
                return "请选择转出课程";
            }
            foreach (var s in TransferCoursesOut)
            {
                var msg = s.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    return msg;
                }
            }
            if (TransferCoursesBuy == null || TransferCoursesBuy.Count == 0)
            {
                return "请选择转入课程";
            }
            foreach (var s in TransferCoursesBuy)
            {
                var msg = s.Validate();
                if (!string.IsNullOrEmpty(msg))
                {
                    return msg;
                }
            }
            if (TransferCoursesOrderInfo == null)
            {
                return "请提交转课信息";
            }

            if (TransferCoursesOrderInfo.InOutType == EmOrderInOutType.In)
            {
                if (TransferCoursesOrderInfo.InPayInfo == null)
                {
                    return "请输入收款信息";
                }
                if (TransferCoursesOrderInfo.InPayInfo.PaySum != TransferCoursesOrderInfo.PaySum)
                {
                    return "支付金额必须等于收款金额";
                }
            }
            if (TransferCoursesOrderInfo.InOutType == EmOrderInOutType.Out && TransferCoursesOrderInfo.OutPayInfo == null)
            {
                return "请输入退款信息";
            }
            return base.Validate();
        }
    }

    public class TransferCoursesOut : IValidate
    {
        public long OrderDetailId { get; set; }

        public long ProductId { get; set; }

        public string OrderNo { get; set; }

        public bool IsAllReturn { get; set; }

        public decimal ReturnCount { get; set; }

        public decimal ReturnSum { get; set; }

        public string Validate()
        {
            if (OrderDetailId <= 0 || ProductId <= 0)
            {
                return "请求数据格式错误";
            }
            if (ReturnCount <= 0)
            {
                return "请输入转出数量";
            }
            return string.Empty;
        }
    }

    public class TransferCoursesBuy : EnrolmentCourse
    {
    }

    public class TransferCoursesOrderInfo
    {
        /// <summary>
        /// 收支类型 <see cref="ETMS.Entity.Enum.EmOrderInOutType"/>
        /// </summary>
        public byte InOutType { get; set; }

        /// <summary>
        /// 收入类型
        /// </summary>
        public InPayInfo InPayInfo { get; set; }

        /// <summary>
        /// 支出信息
        /// </summary>
        public OutPayInfo OutPayInfo { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal PaySum { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public DateTime Ot { get; set; }

        public string Remark { get; set; }

        public int ChangePoint { get; set; }

        public List<long> CommissionUser { get; set; }
    }

    /// <summary>
    /// 支付信息
    /// </summary>
    public class InPayInfo
    {
        public decimal PayWechat { get; set; }

        public decimal PayAlipay { get; set; }

        public decimal PayCash { get; set; }

        public decimal PayBank { get; set; }

        public decimal PayPos { get; set; }

        public decimal PayOther { get; set; }

        public decimal PayAccountRechargeReal { get; set; }

        public decimal PayAccountRechargeGive { get; set; }

        public long? PayAccountRechargeId { get; set; }

        public decimal PaySum
        {
            get
            {
                var temp = PayWechat + PayAlipay + PayCash + PayBank + PayPos + PayOther;
                if (PayAccountRechargeId != null)
                {
                    temp += PayAccountRechargeReal + PayAccountRechargeGive;
                }
                return temp;
            }
        }
    }

    /// <summary>
    /// 支出信息
    /// </summary>
    public class OutPayInfo
    {
        /// <summary>
        /// 支付类型 <see cref="ETMS.Entity.Enum.EmPayType"/>
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 充值账户ID
        /// </summary>
        public long? PayStudentAccountRechargeId { get; set; }
    }
}
