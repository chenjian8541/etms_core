using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Enum
{
    public struct EmMicroWebColumnType
    {
        /// <summary>
        /// 列表
        /// </summary>
        public const byte ListPage = 0;

        /// <summary>
        /// 单页
        /// </summary>
        public const byte SinglePage = 1;

        public static string GetMicroWebColumnTypeDesc(byte t)
        {
            return t == ListPage ? "列表" : "单页";
        }
    }
}
