using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Interaction.Output
{
    public class MicroWebTenantAddressGetOutput
    {
        public string CoverIcon { get; set; }

        public string CoverIconUrl { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public bool IsShowInHome { get; set; }
    }
}
