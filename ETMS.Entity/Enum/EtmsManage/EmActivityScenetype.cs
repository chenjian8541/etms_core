using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmActivityScenetype
    {
        /// <summary>
        /// 热门活动
        /// </summary>
        public const int PopularActivities = 0;

        /// <summary>
        /// 体验课
        /// </summary>
        public const int ExperienceClass = 1;

        /// <summary>
        /// 节假日
        /// </summary>
        public const int Holidays = 2;

        /// <summary>
        /// 春招
        /// </summary>
        public const int SpringRecruit = 3;

        /// <summary>
        /// 夏招
        /// </summary>
        public const int SummerRecruit = 4;

        /// <summary>
        /// 秋招
        /// </summary>
        public const int AutumnRecruit = 5;

        /// <summary>
        /// 冬招
        /// </summary>
        public const int WinterRecruit = 6;

        /// <summary>
        /// 抗疫
        /// </summary>
        public const int AntiEpidemic = 7;

        /// <summary>
        /// 其他
        /// </summary>
        public const int Other = 8;
    }
}
