using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Temp.View
{
    public class MicroWebTenantAddressView
    {
        public string CoverIcon { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public bool IsShowInHome { get; set; }
    }
}
