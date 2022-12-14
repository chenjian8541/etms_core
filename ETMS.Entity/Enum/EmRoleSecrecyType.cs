using ETMS.Entity.View.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public class EmRoleSecrecyType
    {
        /// <summary>
        /// 不限制
        /// </summary>
        public const int NotLimited = 0;

        /// <summary>
        /// 隐藏保护
        /// </summary>
        public const int Secrecy = 1;

        public static string GetSecrecyValue(int secrecyType, SecrecyDataView secrecyDataBag, string value)
        {
            if (secrecyType == EmRoleSecrecyType.Secrecy)
            {
                if (secrecyDataBag == null || !secrecyDataBag.ClassRecordDeSum)
                {
                    return value;
                }
                return "***";
            }
            return value;
        }
    }
}
