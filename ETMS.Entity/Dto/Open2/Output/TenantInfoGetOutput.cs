using ETMS.Entity.Config;
using ETMS.Entity.Dto.BasicData.Output;
using ETMS.Entity.Dto.Interaction.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open2.Output
{
    public class TenantInfoGetOutput
    {
        public GetTenantInfoH5Output TenantInfo { get; set; }

        public MicroWebTenantAddressGetOutput TenantAddressInfo { get; set; }

        public MallGoodsOpenGetConfigOutput MallGoodsConfig { get; set; }

        public TenantConfig TenantConfig { get; set; }
    }
}
