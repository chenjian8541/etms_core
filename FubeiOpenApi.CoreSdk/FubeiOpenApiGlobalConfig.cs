using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Fubei.OpenApi.Sdk
{
    public class FubeiOpenApiGlobalConfig
    {
        public static readonly FubeiOpenApiGlobalConfig Instance = new FubeiOpenApiGlobalConfig();

        /// <summary>
        /// 商户Api地址
        /// </summary>
        public string Api_1_0 { get; set; }

        /// <summary>
        /// 服务商api地址
        /// </summary>
        public string Api_2_0 { get; set; }
    }
}
