using ETMS.Entity.Common;
using ETMS.Entity.View;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.Parent3.Request
{
    public class ParentBuyMallGoodsSubmitRequest : ParentRequestBase
    {
        public string GId { get; set; }

        public long StudentId { get; set; }

        public long Id { get; set; }

        public int BuyCount { get; set; }

        public long? CoursePriceRuleId { get; set; }

        public long? ClassId { get; set; }

        public List<ParentBuyMallGoodsSubmitSpecItem> SpecItems { get; set; }

        public string Remark { get; set; }

        public long TenantLcsPayLogId { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(GId))
            {
                return "请求数据格式错误";
            }
            Id = EtmsHelper2.GetIdDecrypt2(GId);
            if (Id <= 0)
            {
                return "请求数据格式错误";
            }
            if (StudentId <= 0)
            {
                return "请选择学员";
            }
            if (BuyCount <= 0)
            {
                return "购买数量必须大于0";
            }
            if (TenantLcsPayLogId <= 0)
            {
                return "请先支付";
            }
            return base.Validate();
        }
    }
}
