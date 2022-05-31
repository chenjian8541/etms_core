using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Enum
{
    public struct EmClassTimesDataType
    {
        /// <summary>
        /// 正常
        /// </summary>
        public const byte Normal = 0;

        /// <summary>
        /// 暂停
        /// </summary>
        public const byte Stop = 1;

        public static string GetClassTimesDataTypeDesc(byte type)
        {
            return type == Normal ? "正常" : "暂停";
        }
    }
}
