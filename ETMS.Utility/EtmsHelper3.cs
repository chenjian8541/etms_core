using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Utility
{
    public class EtmsHelper3
    {
        public static int GetCent(decimal money)
        {
            return Convert.ToInt32(money * 100);
        }

        public static bool IsEffectiveDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return false;
            }
            if (DateTime.TryParse(date, out var tempDate))
            {
                return tempDate.IsEffectiveDate();
            }
            return false;
        }
    }
}
