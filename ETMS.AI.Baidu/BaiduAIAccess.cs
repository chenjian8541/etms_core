using ETMS.AI.Baidu.Dto.Output;
using ETMS.AI.Baidu.Dto.Request;
using ETMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETMS.AI.Baidu
{
    public class BaiduAIAccess : IBaiduAIAccess
    {
        private readonly IHttpClient _httpClient;

        public BaiduAIAccess(IHttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<LocationOutput> GetLocationInfo(string url)
        {
            return await _httpClient.PostAsync<EmptyRequest, LocationOutput>(url, new EmptyRequest());
        }
    }
}
