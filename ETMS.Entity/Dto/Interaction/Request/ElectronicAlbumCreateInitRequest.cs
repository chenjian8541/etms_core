using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ElectronicAlbumCreateInitRequest : RequestBase
    {
        public byte? Type { get; set; }

        public long RelatedId { get; set; }

        public string Name { get; set; }

        public long TemplateId { get; set; }

        public override string Validate()
        {
            if (TemplateId <= 0)
            {
                return "请选择模板";
            }
            if (Type <= 0)
            {
                return "请求数据格式错误";
            }
            if (RelatedId <= 0)
            {
                return "请选择关联对象";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入相册标题";
            }
            return string.Empty;
        }
    }
}
