using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Request
{
    public class ParentCouponsReceiveRequest : ParentRequestBase
    {
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public long CId { get; set; }

        public long StudentId { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
