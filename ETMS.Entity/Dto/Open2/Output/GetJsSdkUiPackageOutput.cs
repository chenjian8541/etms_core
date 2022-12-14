using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.Open2.Output
{
    public class GetJsSdkUiPackageOutput
    {
        public bool IsSuccess { get; set; }

        public GetJsSdkUiPackageData MyData { get; set; }
    }

    public class GetJsSdkUiPackageData
    {
        public string AppId { get; set; }

        public string Timestamp { get; set; }

        public string NonceStr { get; set; }

        public string Signature { get; set; }
    }
}
