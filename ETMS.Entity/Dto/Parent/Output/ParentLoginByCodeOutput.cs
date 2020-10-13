using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Parent.Output
{
    public class ParentLoginByCodeOutput
    {
        /// <summary>
        /// <see cref="ParentLoginByCodeType"/>
        /// </summary>
        public byte Type { get; set; }

        public long StudentWechartId { get; set; }

        public ParentLoginBySmsOutput LoginInfo { get; set; }
    }

    public struct ParentLoginByCodeType
    {

        /// <summary>
        /// 失败
        /// </summary>
        public const byte Failure = 0;

        /// <summary>
        /// 成功
        /// </summary>
        public const byte Success = 1;
    }
}
