using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmDataLimitType
    {
        /// <summary>
        /// 所有
        /// </summary>
        public const string All = "0";

        /// <summary>
        /// 限制
        /// </summary>
        public const string Limit = "1";

        public static string GetAuthorityValueData(bool isDataLimit)
        {
            return isDataLimit ? EmDataLimitType.Limit : EmDataLimitType.All;
        }

        public static bool GetIsDataLimit(string authorityValueData)
        {
            return authorityValueData.Equals(EmDataLimitType.Limit);
        }
    }
}
