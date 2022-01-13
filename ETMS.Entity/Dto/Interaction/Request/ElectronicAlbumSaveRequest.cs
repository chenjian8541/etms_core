using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ElectronicAlbumSaveRequest : ElectronicAlbumEditOrPublishRequest
    {
        public override string Validate()
        {
            if (string.IsNullOrEmpty(TempIdNo) && string.IsNullOrEmpty(CIdNo))
            {
                return "请求数据格式错误";
            }
            if (string.IsNullOrEmpty(RenderKey))
            {
                return "相册内容不能为空";
            }
            return base.Validate();
        }
    }
}
