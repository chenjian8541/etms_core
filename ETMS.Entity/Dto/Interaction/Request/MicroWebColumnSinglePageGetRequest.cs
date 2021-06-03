using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebColumnSinglePageGetRequest : RequestBase
    {
        public long ColumnId { get; set; }

        public override string Validate()
        {
            if (ColumnId <= 0)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
