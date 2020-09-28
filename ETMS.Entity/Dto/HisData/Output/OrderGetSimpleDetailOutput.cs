using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.HisData.Output
{
    public class OrderGetSimpleDetailOutput
    {
        public OrderGetDetailBascInfo BascInfo { get; set; }

        public List<OrderGetDetailIncomeLog> OrderGetDetailIncomeLogs { get; set; }
    }
}
