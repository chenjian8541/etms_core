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
        /// 展示模板
        /// </summary>
        public const byte ShowTemplate = 1;

        public static string GetShareTemplateTypeDesc(byte t)
        {
            if (t == Link)
            {
                return "分享链接";
            }
            return "展示模板";
        }
    }
}
