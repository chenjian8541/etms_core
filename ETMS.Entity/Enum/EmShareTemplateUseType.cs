using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmShareTemplateUseType
    {
        /// <summary>
        /// 成长档案
        /// </summary>
        public const int Growth = 0;

        /// <summary>
        /// 课后点评
        /// </summary>
        public const int ClassEvaluate = 1;

        /// <summary>
        /// 电子相册
        /// </summary>
        public const int StudentPhoto = 2;

        /// <summary>
        /// 微官网(分享链接)
        /// </summary>
        public const int MicWebsite = 5;

        /// <summary>
        /// 在线商城(分享链接)
        /// </summary>
        public const int OnlineMall = 6;

        public static string GetShareTemplateUseTypeDesc(int t)
        {
            switch (t)
            {
                case Growth:
                    return "成长档案";
                case ClassEvaluate:
                    return "课后点评";
                case StudentPhoto:
                    return "电子相册";
                case MicWebsite:
                    return "微官网";
                case OnlineMall:
                    return "在线商城";
            }
            return string.Empty;
        }
    }
}
