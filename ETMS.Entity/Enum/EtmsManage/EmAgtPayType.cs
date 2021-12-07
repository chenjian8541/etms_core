using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum.EtmsManage
{
    public struct EmAgtPayType
    {
        /// <summary>
        /// 未申请
        /// </summary>
        public const int NotApplied = 0;

        /// <summary>
        /// 利楚扫呗 
        /// https://www.lcsw.cn/doc/guide/docking.html
        /// </summary>
        public const int Lcsw = 1;

        /// <summary>
        /// 付呗
        /// https://www.yuque.com/51fubei/openapi
        /// </summary>
        public const int Fubei = 2;
    }
}
