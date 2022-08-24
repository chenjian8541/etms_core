using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.AI.Baidu.Dto.Output
{
    public class LocationOutput
    {
        public string status { get; set; }

        public string address { get; set; }

        public LocationContent content { get; set; }

    }

    public class LocationContent
    {
        public address_detail address_detail { get; set; }

        public point point { get; set; }

        public string address { get; set; }
    }

    public class address_detail
    {
        public string province { get; set; }

        public string city { get; set; }

        public string district { get; set; }

        public string street_number { get; set; }

        public string adcode { get; set; }

        public string street { get; set; }

        public string city_code { get; set; }
    }

    public class point
    {
        public string x { get; set; }

        public string y { get; set; }
    }
}
