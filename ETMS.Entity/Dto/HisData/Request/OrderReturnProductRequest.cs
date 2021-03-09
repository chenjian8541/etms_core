using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Request
{
    public class OrderReturnProductRequest : RequestBase
    {
        public long ReturnOrderId { get; set; }

        public OrderReturnOrderInfo OrderReturnOrderInfo { get; set; }

        public List<OrderReturnProductItem> OrderReturnProductItems { get; set; }

        public override string Validate()
        {
            if (ReturnOrderId <= 0)
            {
                return "请求数据不合法";
            }
            if (OrderReturnOrderInfo == null)
            {
                return "请提交退单信息";
            }
            if (OrderReturnProductItems == null || OrderReturnProductItems.Count == 0)
            {
                return "请选择需要退的项目";
            }
            foreach (var p in OrderReturnProductItems)
            {
                var errMsg = p.Validate();
                if (!string.IsNullOrEmpty(errMsg))
                {
                    return errMsg;
                }
            }
            return string.Empty;
        }
    }

    public class OrderReturnOrderInfo
    {
        /// <summary>
        /// 支付类型 <see cref="ETMS.Entity.Enum.EmPayType"/>
        /// </summary>
        public int PayType { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal PaySum { get; set; }

        /// <summary>
        /// 经办日期
        /// </summary>
        public DateTime Ot { get; set; }

        public string Remark { get; set; }

        public int DePoint { get; set; }

        public long? PayStudentAccountRechargeId { get; set; }
    }

    public class OrderReturnProductItem : IValidate
    {
        public long OrderDetailId { get; set; }

        public long ProductId { get; set; }

        public string ProductName { get; set; }

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
                return "请输入退出数量";
            }
            return string.Empty;
        }
    }
}