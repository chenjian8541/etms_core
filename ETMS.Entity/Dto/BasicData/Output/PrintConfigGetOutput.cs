using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class PrintConfigGetOutput
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

        /// <summary>
        /// 二维码图片
        /// </summary>
        public string TagImg { get; set; }
    }
}
