using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class ElectronicAlbumPageInitRequest : OpenLinkBase
    {
        public string TempIdNo { get; set; }

        public string CIdNo { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(TempIdNo) && string.IsNullOrEmpty(CIdNo))
            {
                return "请求数据格式错误";
            }
            return base.Validate();
        }
    }
}
