using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Common.Request
{
    public class UploadImg2Request : RequestBase
    {
        public string ImgData { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(ImgData))
            {
                return "图片信息不正确";
            }
            return base.Validate();
        }
    }
}
