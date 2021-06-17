using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open2.Request
{
    public class TryCalssApplySupplementRequest : Open2Base
    {
        public long TryClassId { get; set; }

        public string Name { get; set; }

        public string TryCourse { get; set; }

        public override string Validate()
        {
            if (TryClassId <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入学员名称";
            }
            return base.Validate();
        }
    }
}
