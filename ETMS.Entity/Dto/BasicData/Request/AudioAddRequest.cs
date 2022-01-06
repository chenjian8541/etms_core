using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class AudioAddRequest : RequestBase
    {
        public int Type { get; set; }

        public string Name { get; set; }

        public string AudioKey { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(AudioKey))
            {
                return "请上传图片";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入名称";
            }
            return string.Empty;
        }
    }
}
