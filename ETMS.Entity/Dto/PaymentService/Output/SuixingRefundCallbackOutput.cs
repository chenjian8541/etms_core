using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class SuixingRefundCallbackOutput
    {
        public string code { get; set; }

        public string msg { get; set; }

        public static SuixingRefundCallbackOutput Success()
        {
            return new SuixingRefundCallbackOutput()
            {
                code = "success",
                msg = "成功"
            };
        }

        public static SuixingRefundCallbackOutput Fail()
        {
            return new SuixingRefundCallbackOutput()
            {
                code = "fail",
                msg = "失败"
            };
        }
    }
}
