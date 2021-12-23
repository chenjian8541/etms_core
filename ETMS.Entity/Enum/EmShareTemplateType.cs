using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmShareTemplateType
    {
        /// <summary>
        /// 分享链接
        /// </summary>
        public const byte Link = 0;

        /// <summary>
        /// 分享海报
        /// </summary>
        public const byte Poster = 1;

        public static string GetShareTemplateTypeDesc(byte t)
        {
            if (t == Link)
            {
                return "分享链接";
            }
            return "分享海报";
        }
    }
}
