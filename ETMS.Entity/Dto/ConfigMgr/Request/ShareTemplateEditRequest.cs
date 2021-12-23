using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETMS.Entity.Enum;
namespace ETMS.Entity.Dto.ConfigMgr.Request
{
    public class ShareTemplateEditRequest : RequestBase
    {
        public long Id { get; set; }

        public string ImgKey { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public override string Validate()
        {
            if (Id <= 0)
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入名称";
            }
            if (string.IsNullOrEmpty(Title))
            {
                return "请输入标题";
            }
            if (string.IsNullOrEmpty(ImgKey))
            {
                return "请选择图片";
            }
            return string.Empty;
        }
    }
}
