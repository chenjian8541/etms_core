using ETMS.Entity.Common;
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
                return "请求数据不合法";
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
                return "请求数据不合法";
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
        /// 支出类型 <see cref="ETMS.Entity.Enum.EmOrderInOutType"/>
        /// </summary>
        public byte InOutType { get; set; }

        /// <summary>
        /// 支付类型 <see cref="ETMS.Entity.Enum.EmPayType"/>
        /// </summary>
        public byte PayType { get; set; }

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
}
