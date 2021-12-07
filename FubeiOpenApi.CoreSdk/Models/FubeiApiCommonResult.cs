using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Com.Fubei.OpenApi.Sdk.Models
{
    /// <summary>
    /// 返回的通用结果
    /// </summary>
    /// <typeparam name="TData">Data实体类型</typeparam>
    public class FubeiApiCommonResult<TData> : BaseEntity
    {
        [JsonProperty("result_code")]
        public string ResultCode { get; set; }

        [JsonProperty("result_message")]
        public string ResultMessage { get; set; }

        [JsonProperty("data")]
        public TData Data { get; set; }

        public bool IsSuccess()
        {
            return this.ResultCode == "200";
        }
    }
}
