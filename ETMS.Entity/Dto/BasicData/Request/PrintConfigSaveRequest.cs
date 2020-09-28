using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class PrintConfigSaveRequest : RequestBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 宣传语
        /// </summary>
        public string BottomDesc { get; set; }

        /// <summary>
        /// 打印类型  <see cref=" ETMS.Entity.Enum.EmPrintType"/>
        /// </summary>
        public int PrintType { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        public string TagImgKey { get; set; }
    }
}
