using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class ImageListGetRequest : RequestBase
    {
        public int? Type { get; set; }

        public override string Validate()
        {
            if (Type==null)
            {
                return "请求数据格式错误";
            }
            return string.Empty;
        }
    }
}
