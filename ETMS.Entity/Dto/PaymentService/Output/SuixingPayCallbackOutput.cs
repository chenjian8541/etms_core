using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class SuixingPayCallbackOutput
    {
        public string code { get; set; }

        public string msg { get; set; }

        public static SuixingPayCallbackOutput Success()
        {
            return new SuixingPayCallbackOutput()
            {
                code = "success",
                msg = "成功"
            };
        }

        public static SuixingPayCallbackOutput Fail()
        {
            return new SuixingPayCallbackOutput()
            {
                code = "fail",
                msg = "失败"
            };
        }
    }
}
