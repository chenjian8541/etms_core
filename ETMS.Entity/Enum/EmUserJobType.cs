using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    /// <summary>
    /// 在职类型
    /// </summary>
    public struct EmUserJobType
    {
        /// <summary>
        /// 全职
        /// </summary>
        public const int FullTime = 0;

        /// <summary>
        /// 兼职
        /// </summary>
        public const int PartTime = 1;

        /// <summary>
        /// 离职
        /// </summary>
        public const int Resignation = 2;

        public static string GetUserJobTypeDesc(int type)
        {
            switch (type)
            {
                case EmUserJobType.FullTime:
                    return "全职";
                case EmUserJobType.PartTime:
                    return "兼职";
                case EmUserJobType.Resignation:
                    return "离职";
            }
            return string.Empty;
        }
    }
}
