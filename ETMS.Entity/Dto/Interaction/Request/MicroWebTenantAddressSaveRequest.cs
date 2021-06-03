using ETMS.Entity.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Request
{
    public class MicroWebTenantAddressSaveRequest : RequestBase
    {
        public string CoverIcon { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public bool IsShowInHome { get; set; }

        public override string Validate()
        {
            if (string.IsNullOrEmpty(CoverIcon))
            {
                return "请选择地址图标";
            }
            if (string.IsNullOrEmpty(Name))
            {
                return "请输入名称";
            }
            if (string.IsNullOrEmpty(Address))
            {
                return "请输入地址";
            }
            if (string.IsNullOrEmpty(Longitude) || string.IsNullOrEmpty(Latitude))
            {
                return "请输入地址经纬度";
            }
            return base.Validate();
        }
    }
}
