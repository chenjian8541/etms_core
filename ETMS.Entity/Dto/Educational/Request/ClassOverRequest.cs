using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETMS.Entity.Dto.Educational.Request
{
    public class ClassOverRequest : RequestBase
    {
        public List<ClassOverItem> Items { get; set; }

        public override string Validate()
        {
            if (Items == null || !Items.Any())
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }

    public class ClassOverItem
    {
        public string ClassName { get; set; }

        public long CId { get; set; }
    }
}
