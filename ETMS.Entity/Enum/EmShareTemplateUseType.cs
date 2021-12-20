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
        public const byte Growth = 0;

        /// <summary>
        /// 课后点评
        /// </summary>
        public const byte ClassEvaluate = 1;

        /// <summary>
        /// 电子相册
        /// </summary>
        public const byte StudentPhoto = 2;

        /// <summary>
        /// 微官网(分享链接)
        /// </summary>
        public const byte MicWebsite = 5;

        /// <summary>
        /// 在线商城(分享链接)
        /// </summary>
        public const byte OnlineMall = 6;
    }
}
