using ETMS.Entity.Common;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Request
{
    public class ImageAddRequest : RequestBase
    {
        public int Type { get; set; }

        public string ImgKey { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(ImgKey))
            {
                return "请上传图片";
            }
            return string.Empty;
        }
    }
}
