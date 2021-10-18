using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassTimesStudentGetRequest : RequestBase
    {
        public long CId { get; set; }

        /// <summary>
        /// <see cref="ETMS.Entity.Enum.ClassTimesGetStudentType"/>
        /// </summary>
        public int Type { get; set; }

        public override string Validate()
        {
            if (CId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
