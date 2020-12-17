using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.View
{
    public class TencentCloudAccountView
    {
        public int TencentCloudId { get; set; }

        public string SecretId { get; set; }

        public string SecretKey { get; set; }

        public string Endpoint { get; set; }

        public string Region { get; set; }
    }
}
