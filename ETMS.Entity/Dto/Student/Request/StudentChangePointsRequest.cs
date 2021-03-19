using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Student.Request
{
    public class StudentChangePointsRequest : RequestBase
    {
        public long CId { get; set; }

        public byte ChangeType { get; set; }

        public int ChangePoints { get; set; }

        public string Remark { get; set; }
        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            if (ChangePoints <= 0)
            {
                return "请输入变动积分";
            }
            return string.Empty;
        }
    }

    public struct EmChangePointsType
    {
        /// <summary>
        /// 增加
        /// </summary>
        public const byte Add = 0;

        /// <summary>
        /// 扣除
        /// </summary>
        public const byte Deduction = 1;
    }
}
