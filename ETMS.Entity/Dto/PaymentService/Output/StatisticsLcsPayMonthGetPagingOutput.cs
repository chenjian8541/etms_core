using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.PaymentService.Output
{
    public class StatisticsLcsPayMonthGetPagingOutput
    {
        public DateTime Ot { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public decimal TotalMoney { get; set; }

        public decimal TotalMoneyRefund { get; set; }

        public decimal TotalMoneyValue { get; set; }
    }
}
