using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmSystemMenusCategory
    {
        /// <summary>
        /// 业务中心
        /// </summary>
        public const int BusinessCenter = 1;

        /// <summary>
        /// 教务中心
        /// </summary>
        public const int EducationCenter = 2;

        /// <summary>
        /// 家校互动
        /// </summary>
        public const int HomeSchoolInteraction = 3;

        /// <summary>
        /// 营销中心
        /// </summary>
        public const int MarketingCenter = 4;

        /// <summary>
        /// 财务中心
        /// </summary>
        public const int FinancialCenter = 5;

        /// <summary>
        /// 数据中心
        /// </summary>
        public const int DataCenter = 6;

        /// <summary>
        /// 机构设置
        /// </summary>
        public const int Setting = 7;

        /// <summary>
        /// 增值服务
        /// </summary>
        public const int IncreaseServices = 8;

        public static string GetSystemMenusCategoryName(int t)
        {
            switch (t)
            {
                case 1:
                    return "业务中心";
                case 2:
                    return "教务中心";
                case 3:
                    return "家校互动";
                case 4:
                    return "营销中心";
                case 5:
                    return "财务中心";
                case 6:
                    return "数据中心";
                case 7:
                    return "机构设置";
                case 8:
                    return "增值服务";
            }
            return string.Empty;
        }
    }
}
