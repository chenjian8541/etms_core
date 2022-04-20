using ETMS.Entity.Dto.Common;
using ETMS.Entity.Enum;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Business.Common
{
    internal static class ComBusiness5
    {
        public static CompareValue GetCompareValue(decimal startValue, decimal endValue)
        {
            if (startValue == 0 || endValue == 0)
            {
                return new CompareValue()
                {
                    Type = EmCompareValueType.Upward,
                    Desc = "0"
                };
            }
            if (endValue > startValue)
            {
                //增长
                var diff = endValue - startValue;
                var pro = diff / startValue;
                var value = pro.EtmsPercentage2();
                return new CompareValue()
                {
                    Type = EmCompareValueType.Upward,
                    Desc = value
                };
            }
            else
            {
                var diff2 = startValue - endValue;
                var pro2 = diff2 / startValue;
                var value2 = pro2.EtmsPercentage2();
                return new CompareValue()
                {
                    Type = EmCompareValueType.Downward,
                    Desc = value2
                };
            }
        }
    }
}
