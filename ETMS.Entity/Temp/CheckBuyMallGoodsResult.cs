using ETMS.Entity.Database.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Temp
{
    public class CheckBuyMallGoodsResult
    {
        public CheckBuyMallGoodsResult()
        { }

        public CheckBuyMallGoodsResult(string msg)
        {
            this.ErrMsg = msg;
        }

        public string ErrMsg { get; set; }

        public decimal TotalMoney { get; set; }

        public EtMallCoursePriceRule CoursePriceRule { get; set; }
    }
}
